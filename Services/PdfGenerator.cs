using Microsoft.Playwright;

public static class PdfGenerator
{
    public static async Task GeneratePdf(string htmlPath, string pdfPath)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        var page = await browser.NewPageAsync();

        var html = await File.ReadAllTextAsync(htmlPath);
        await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.NetworkIdle });

        await page.PdfAsync(new PagePdfOptions
        {
            Path = pdfPath,
            Format = "A4",
            PrintBackground = true,
            Margin = new() { Top = "20mm", Bottom = "20mm", Left = "15mm", Right = "15mm" }
        });
    }
}