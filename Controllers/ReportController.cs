using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HtmlToPdfWeb.Controllers;

public class ReportController : Controller
{
    public class Row
    {
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
    }

    private readonly IViewRenderService _viewRenderService;

    public ReportController(IViewRenderService viewRenderService)
    {
        _viewRenderService = viewRenderService;
    }

    [HttpGet("/report")]
    public async Task<IActionResult> GenerateReport()
    {
        var tables = new List<List<Row>>();
        var data1 = new List<Row>
        {
            new Row { Col1 = "A", Col2 = "B", Col3 = "C", Col4 = "D" },
            new Row { Col1 = "E", Col2 = "F", Col3 = "G", Col4 = "H" }
        };
         var data2 = new List<Row>
        {
            new Row { Col1 = "I", Col2 = "J", Col3 = "K", Col4 = "L" },
            new Row { Col1 = "M", Col2 = "N", Col3 = "O", Col4 = "P" }
        };

        tables.Add(data1);
        tables.Add(data2);

        var html = await _viewRenderService.RenderToStringAsync("Report/ReportTemplate", tables);

        var htmlPath = Path.GetTempFileName() + ".html";
        await System.IO.File.WriteAllTextAsync(htmlPath, html);

        var outputPath = Path.ChangeExtension(htmlPath, ".pdf");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = $"render.js {htmlPath} {outputPath}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        if (System.IO.File.Exists(outputPath))
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(outputPath);
            return File(bytes, "application/pdf", "report.pdf");
        }

        return BadRequest("PDF-Generierung fehlgeschlagen");
    }
}