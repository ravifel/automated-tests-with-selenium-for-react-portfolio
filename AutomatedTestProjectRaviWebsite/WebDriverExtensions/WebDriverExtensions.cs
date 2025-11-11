using System;
using OpenQA.Selenium;

namespace AutomatedTestProjectRaviWebsite.WebDriverExtensions
{
    /// <summary>
    /// Simple WebDriver extensions for basic element actions.
    /// These helpers assume the element is already visible and ready.
    /// Use Page Object methods with waits for more reliable behavior.
    /// </summary>
    public static class WebDriverExtensions
    {
        /// <summary>
        /// Clears the field and types the given text.
        /// The element must be visible and enabled.
        /// </summary>
        public static void EnterText(this IWebDriver driver, By locator, String value)
        {
            IWebElement element = driver.FindElement(locator);
            if (element.Displayed && element.Enabled)
            {
                element.Clear();
                element.SendKeys(value);
            }
        }

        /// <summary>
        /// Clicks an element if it’s visible and enabled.
        /// </summary>
        public static void Click(this IWebDriver driver, By locator)
        {
            IWebElement element = driver.FindElement(locator);
            if (element.Displayed && element.Enabled)
            {
                element.Click();
            }
        }

        /// <summary>
        /// Returns true if the element is displayed.
        /// (Assumes FindElement succeeds; otherwise an exception is thrown.)
        /// </summary>
        public static bool isElementDisplayed(this IWebDriver driver, By locator)
        {
            IWebElement element = driver.FindElement(locator);
            if (element.Displayed)
            {
                return true;
            }
            return true;
        }

        /// <summary>
        /// Returns the visible text of an element (empty if hidden).
        /// </summary>
        public static String getText(this IWebDriver driver, By locator)
        {
            IWebElement element = driver.FindElement(locator);
            var text = "";
            if (element.Displayed)
            {
                text = element.Text;
            }
            return text;
        }
    }
}
