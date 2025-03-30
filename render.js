const puppeteer = require('puppeteer');
const fs = require('fs');

(async () => {
  const browser = await puppeteer.launch({ headless: 'new' });
  const page = await browser.newPage();

  const html = fs.readFileSync(process.argv[2], 'utf8');
  await page.setContent(html, { waitUntil: 'networkidle0' });

  await page.pdf({
    path: process.argv[3],
    format: 'A4',
    printBackground: true,
    displayHeaderFooter: false,
    margin: { top: '40px', bottom: '40px' }
  });

  await browser.close();
})();