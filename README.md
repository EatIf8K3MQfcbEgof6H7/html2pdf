## run after first install of Playwright
    playwright install chromium


## in docker
    RUN dotnet restore
    RUN dotnet tool install --global Microsoft.Playwright.CLI
    RUN playwright install --with-deps
