using System;
using System.Linq;
using OpenQA.Selenium;

namespace AutomatedTestProjectRaviWebsite.Utils
{
    /// <summary>
    /// Window and tab helper methods for Selenium WebDriver.
    /// Provides an extension to switch to a newly opened window within a timeout.
    /// </summary>
    public static class WindowHelpers
    {
        /// <summary>
        /// Switches to a new tab/window if one opens within the specified timeout.
        /// Example:
        ///   var current = driver.CurrentWindowHandle;
        ///   // perform action that opens a new tab
        ///   driver.SwitchToNewWindowIfAny(current, 5000);
        ///
        /// If no new window appears, the driver stays on the current one.
        /// </summary>
        /// <param name="driver">The active WebDriver instance.</param>
        /// <param name="currentHandle">The handle of the currently active window before the action.</param>
        /// <param name="timeoutMs">Maximum wait time (milliseconds) for a new window to appear.</param>
        public static void SwitchToNewWindowIfAny(this IWebDriver driver, string currentHandle, int timeoutMs = 5000)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            var initialCount = driver.WindowHandles.Count;

            // Wait until the number of windows changes or timeout expires
            while (DateTime.UtcNow < deadline && driver.WindowHandles.Count == initialCount)
                System.Threading.Thread.Sleep(100);

            // If a new window exists, switch to the first handle that isn’t the current one
            if (driver.WindowHandles.Count > initialCount)
            {
                var newHandle = driver.WindowHandles.First(h => h != currentHandle);
                driver.SwitchTo().Window(newHandle);
            }
        }
    }
}
