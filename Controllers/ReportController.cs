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

        var pdfPath = Path.ChangeExtension(htmlPath, ".pdf");

        await PdfGenerator.GeneratePdf(htmlPath, pdfPath);

        return File(System.IO.File.ReadAllBytes(pdfPath), "application/pdf", "report.pdf");

    }
}