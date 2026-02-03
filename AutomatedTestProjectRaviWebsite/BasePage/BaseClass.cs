using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Diagnostics;

namespace AutomatedTestsRaviPortfolio.BasePage
{
    /// <summary>
    /// Test bootstrap class.
    /// Provides a shared WebDriver instance and common setup/teardown for all tests that inherit from it.
    /// </summary>
    public class BaseClass
    {
        /// <summary>
        /// Exposed to derived test classes so they can use the driver directly.
        /// </summary>
        protected IWebDriver driver;

        /// <summary>
        /// Runs BEFORE each test.
        /// - Kills orphaned driver processes (prevents locks/port conflicts).
        /// - Creates a browser instance based on the BROWSER environment variable (chrome|edge|firefox).
        /// - Applies a defensive page-load timeout.
        /// </summary>
        [SetUp]
        public void Init()
        {
            // Ensure no leftover driver processes are holding file locks.
            KillDriverProcesses();

            /*
             * Browser selection
             * -----------------
             * Default: Google Chrome.
             *
             * To run in a different browser, set the BROWSER environment variable
             * to "edge" or "firefox" BEFORE running the tests.
             *
             * Examples:
             * 1) Windows (cmd):
             *    set BROWSER=firefox
             *    dotnet test
             *
             * 2) Linux/Mac (bash/zsh):
             *    export BROWSER=edge
             *    dotnet test
             *
             * 3) Visual Studio:
             *    Test > Configure Run Settings > ... > Environment Variables
             *    Add BROWSER with "chrome", "edge", or "firefox".
             *
             * 4) CI/CD (GitHub Actions / Azure DevOps):
             *    Add BROWSER to the pipeline environment.
             */

            string browser = (Environment.GetEnvironmentVariable("BROWSER") ?? "chrome").ToLower();
            bool headless = (Environment.GetEnvironmentVariable("HEADLESS") ?? "false").ToLower() == "true";

            switch (browser)
            {
                case "firefox":
                    var firefoxOptions = new FirefoxOptions();
                    if (headless) firefoxOptions.AddArgument("--headless");
                    firefoxOptions.AddArgument("--width=1920");
                    firefoxOptions.AddArgument("--height=1080");
                    driver = new FirefoxDriver(firefoxOptions);
                    break;

                case "edge":
                    var edgeOptions = new EdgeOptions();
                    if (headless) edgeOptions.AddArgument("--headless=new");
                    edgeOptions.AddArgument("--window-size=1920,1080");
                    driver = new EdgeDriver(edgeOptions);
                    break;

                case "chrome":
                default:
                    var chromeOptions = new ChromeOptions();
                    if (headless) chromeOptions.AddArgument("--headless=new");
                    chromeOptions.AddArgument("--window-size=1920,1080");

                    // Improves stability in CI/containerized environments.
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");

                    driver = new ChromeDriver(chromeOptions);
                    break;
            }

            // PageLoad timeout: how long Selenium waits for the full page load event.
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
        }

        /// <summary>
        /// Runs AFTER each test.
        /// Safely quits and disposes the WebDriver to release browser processes and OS resources.
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                if (status == NUnit.Framework.Interfaces.TestStatus.Failed && driver is ITakesScreenshot ss)
                {
                    var dir = System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts");
                    System.IO.Directory.CreateDirectory(dir);

                    var file = System.IO.Path.Combine(dir, $"{TestContext.CurrentContext.Test.Name}.png");
                    ss.GetScreenshot().SaveAsFile(file);
                    TestContext.AddTestAttachment(file, "Screenshot on failure");
                }
            }
            catch { /* não quebra o teardown */ }

            if (driver != null)
            {
                try { driver.Quit(); driver.Dispose(); }
                catch (Exception e) { Console.WriteLine("Error while disposing WebDriver: " + e.Message); }
            }
        }

        /// <summary>
        /// Kills orphaned driver processes that may keep binaries locked
        /// (chromedriver/msedgedriver/geckodriver). Use with care on shared machines.
        /// </summary>
        private void KillDriverProcesses()
        {
            // Note: this kills ALL processes with these names on the machine.
            // In shared environments/CI, consider narrowing the scope if needed.
            string[] processNames = new[] { "chromedriver", "msedgedriver", "geckodriver" };

            foreach (var processName in processNames)
            {
                foreach (var process in Process.GetProcessesByName(processName))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to kill process '" + processName + "': " + ex.Message);
                    }
                }
            }
        }
    }
}
