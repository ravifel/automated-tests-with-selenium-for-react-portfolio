using System;
using OpenQA.Selenium;

namespace AutomatedTestProjectRaviWebsite.Utils
{
    /// <summary>
    /// UI helpers shared by tests and page objects:
    /// - Theme detection/forcing (Bootstrap + fallbacks)
    /// - HTML5 validation helpers
    /// - Effective background color sampling
    /// - Lightweight polling utility
    /// </summary>
    public static class UiHelpers
    {
        /// <summary>
        /// Detects the active theme ("dark" or "light").
        /// Order:
        /// 1) data-bs-theme on <html>/<body>
        /// 2) localStorage keys: theme/color-scheme/preferredTheme
        /// 3) Fallback: 'dark' class on <html>/<body>
        /// </summary>
        public static string GetTheme(IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;

            // 1) Bootstrap: data-bs-theme on <html>/<body>
            var bsHtml = js.ExecuteScript("return document.documentElement.getAttribute('data-bs-theme')")?.ToString();
            var bsBody = js.ExecuteScript("return document.body.getAttribute('data-bs-theme')")?.ToString();
            var val = bsHtml ?? bsBody;
            if (!string.IsNullOrWhiteSpace(val)) return val.ToLowerInvariant();

            // 2) localStorage (common keys)
            var ls = js.ExecuteScript(@"
                const keys = ['theme','color-scheme','preferredTheme'];
                for (const k of keys) {
                  const v = window.localStorage.getItem(k);
                  if (v) return v.toString();
                }
                return null;
            ")?.ToString();
            if (!string.IsNullOrWhiteSpace(ls))
            {
                var v = ls.ToLowerInvariant();
                if (v.Contains("dark")) return "dark";
                if (v.Contains("light")) return "light";
            }

            // 3) Fallback: 'dark' class on <html>/<body>
            var hasDarkHtml = (bool)js.ExecuteScript("return document.documentElement.classList.contains('dark')");
            var hasDarkBody = (bool)js.ExecuteScript("return document.body.classList.contains('dark')");
            return (hasDarkHtml || hasDarkBody) ? "dark" : "light";
        }

        /// <summary>
        /// Forces theme ("dark" or "light"):
        /// - sets data-bs-theme on <html>/<body>
        /// - sets localStorage("theme")
        /// - syncs 'dark'/'light' classes on <html>/<body>
        /// </summary>
        public static void SetTheme(IWebDriver driver, string theme)
        {
            var js = (IJavaScriptExecutor)driver;
            theme = (theme ?? "light").ToLowerInvariant().Contains("dark") ? "dark" : "light";

            js.ExecuteScript(@"
                document.documentElement.setAttribute('data-bs-theme', arguments[0]);
                document.body.setAttribute('data-bs-theme', arguments[0]);
                try { window.localStorage.setItem('theme', arguments[0]); } catch(e) {}
                
                document.documentElement.classList.remove('dark','light');
                document.body.classList.remove('dark','light');
                if (arguments[0] === 'dark') {
                    document.documentElement.classList.add('dark');
                    document.body.classList.add('dark');
                } else {
                    document.documentElement.classList.add('light');
                    document.body.classList.add('light');
                }
            ", theme);
        }

        /// <summary>
        /// Polls until the theme equals <paramref name="expected"/> or times out.
        /// </summary>
        public static void WaitForThemeToBe(IWebDriver driver, string expected, int timeoutMs = 3000)
        {
            expected = (expected ?? "").ToLowerInvariant();
            var end = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < end)
            {
                var now = GetTheme(driver).ToLowerInvariant();
                if (now == expected) return;
                System.Threading.Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Polls until the theme changes from <paramref name="before"/> or times out.
        /// </summary>
        public static void WaitForThemeToChange(IWebDriver driver, string before, int timeoutMs = 3000)
        {
            before = (before ?? "").ToLowerInvariant();
            var end = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < end)
            {
                var now = GetTheme(driver).ToLowerInvariant();
                if (now != before) return;
                System.Threading.Thread.Sleep(50);
            }
        }

        /// <summary>
        /// HTML5: returns true if the element is INVALID (checkValidity() == false).
        /// </summary>
        public static bool IsInvalid(IWebDriver driver, By by)
        {
            var el = driver.FindElement(by);
            var js = (IJavaScriptExecutor)driver;
            var ok = (bool)js.ExecuteScript("return arguments[0].checkValidity()", el);
            return !ok;
        }

        /// <summary>
        /// HTML5 validity details: valueMissing, typeMismatch, validationMessage.
        /// </summary>
        public static (bool valueMissing, bool typeMismatch, string message) GetValidity(IWebDriver driver, By by)
        {
            var el = driver.FindElement(by);
            var js = (IJavaScriptExecutor)driver;
            var miss = (bool)js.ExecuteScript("return arguments[0].validity.valueMissing", el);
            var type = (bool)js.ExecuteScript("return arguments[0].validity.typeMismatch", el);
            var msg = (string)js.ExecuteScript("return arguments[0].validationMessage", el);
            return (miss, type, msg);
        }

        /// <summary>
        /// Triggers the native validation bubble (if available).
        /// </summary>
        public static void ReportValidity(IWebDriver driver, By by)
        {
            var el = driver.FindElement(by);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].reportValidity()", el);
        }

        /// <summary>
        /// Returns the effective background color by sampling the viewport center
        /// and walking up until a non-transparent background is found.
        /// </summary>
        public static string GetEffectiveBgColor(IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;
            var color = js.ExecuteScript(@"
                const x = Math.floor(window.innerWidth/2);
                const y = Math.floor(window.innerHeight/2);
                let el = document.elementFromPoint(x, y) || document.body;
                const transparent = ['rgba(0, 0, 0, 0)', 'transparent'];
                while (el) {
                    const bg = window.getComputedStyle(el).backgroundColor;
                    if (bg && !transparent.includes(bg)) return bg;
                    el = el.parentElement;
                }
                return window.getComputedStyle(document.body).backgroundColor;
            ");
            return color?.ToString() ?? "";
        }

        /// <summary>
        /// Polls a condition until true or timeout.
        /// </summary>
        public static void WaitUntil(Func<bool> condition, int timeoutMs = 3000, int pollMs = 50)
        {
            var end = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < end)
            {
                try { if (condition()) return; } catch { /* ignore and keep polling */ }
                System.Threading.Thread.Sleep(pollMs);
            }
        }
    }
}
