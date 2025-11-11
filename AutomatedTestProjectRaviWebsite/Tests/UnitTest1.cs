using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Assert = NUnit.Framework.Assert;

namespace AutomatedTestsRaviPortfolio.Tests
{
    /// <summary>
    /// Basic smoke test for the portfolio site.
    /// This class does NOT inherit from BaseClass — it manages its own WebDriver instance.
    /// </summary>
    public class PortfolioBasicTest
    {
        private IWebDriver _driver;

        /// <summary>
        /// Runs before each test.
        /// Creates a fresh ChromeDriver instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
        }

        /// <summary>
        /// Verifies the portfolio page loads successfully and the title contains the expected text.
        /// </summary>
        [Test]
        public void GivenPageDisplayed_WhenLoadedInTheBrowser_ThenThePageTitleIsDisplayed()
        {
            // Navigate to the portfolio page
            _driver.Navigate().GoToUrl("https://ravifel.github.io/");

            // Expected and actual page titles
            string expectedTitle = "Ravi Professional Portfolio";
            string actualTitle = _driver.Title;

            // Assertion (message kept in Portuguese intentionally)
            Assert.IsTrue(
                actualTitle.Contains(expectedTitle),
                $"O título da página deveria conter '{expectedTitle}', mas foi: {actualTitle}"
            );
        }

        /// <summary>
        /// Runs after each test and closes the browser.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _driver.Dispose();
        }
    }
}
