using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using AutomatedTestProjectRaviWebsite.Utils;

namespace AutomatedTestsRaviPortfolio.Pages
{
    /// <summary>
    /// Page Object for the Home page.
    /// Centralizes element locators, robust UI actions (click/type/waits),
    /// small workflows (contact form, language switch), theme checks,
    /// and helpers for validating external links.
    /// </summary>
    public class HomePage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        /// <summary>
        /// Injects WebDriver and configures a default WebDriverWait used across this page.
        /// </summary>
        public HomePage(IWebDriver driver, int waitSeconds = 15)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitSeconds));
        }

        // ==============================
        // ELEMENT MAPPING
        // Keep all locators in one place for readability and easy maintenance.
        // ==============================
        public By BtnNavBarBrand => By.Id("navbar-brand");
        public By BtnNavBarHome => By.Id("link-home");
        public By BtnNavBarRepositories => By.Id("link-repositories");
        public By BtnNavBarTestimonials => By.Id("link-testimonials");
        public By BtnNavBarThemeDarkLight => By.Id("toggle-theme-btn");
        public By SelectorBtnNavBarLanguage => By.Id("language-select");

        public By BtnCoverLetter => By.Id("btn-cover-letter-en");
        public By BtnResumeEng => By.Id("btn-resume-en");
        public By BtnContactForm => By.Id("btn-contact-email");
        public By BtnContactWhatsapp => By.Id("btn-contact-whatsapp");
        public By BtnContactLinkedin => By.Id("btn-contact-linkedin");
        public By BtnContactGitHub => By.Id("btn-contact-github");

        public By InputNameContactForm => By.Id("input-name");
        public By InputEmailContactForm => By.Id("input-email");
        public By InputMessageContactForm => By.Id("input-message");
        public By BtnContactFormSend => By.Id("btn-contact-send");
        public By BtnContactFormClose => By.XPath("//button[@aria-label='Close']");

        // ==============================
        // GENERIC UTILITIES
        // Reusable actions and waits that make tests more reliable.
        // ==============================

        /// <summary>
        /// Navigates to the site root and waits for the page to fully load.
        /// </summary>
        public void Open()
        {
            driver.Navigate().GoToUrl("https://ravifel.github.io/");
            WaitForPageLoad();
        }

        /// <summary>
        /// Robust click:
        /// 1) wait until clickable,
        /// 2) scroll into viewport center,
        /// 3) try native click, then JS click as a fallback.
        /// </summary>
        public void Click(By locator)
        {
            var element = wait.Until(ExpectedConditions.ElementToBeClickable(locator));

            // Scroll to center to avoid overlays (headers/footers).
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "arguments[0].scrollIntoView({block:'center', inline:'center'});", element);

            try
            {
                element.Click();
            }
            catch (ElementClickInterceptedException)
            {
                // If briefly covered, fallback to JS click.
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }

        /// <summary>
        /// Safe type into input: wait visible, clear, then send keys.
        /// </summary>
        public void FillField(By locator, string value)
        {
            var element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
            element.Clear();
            element.SendKeys(value);
        }

        /// <summary>
        /// Waits for visibility with optional custom timeout.
        /// </summary>
        public IWebElement WaitForElementVisible(By locator, int? customTimeout = null)
        {
            var customWait = customTimeout.HasValue
                ? new WebDriverWait(driver, TimeSpan.FromSeconds(customTimeout.Value))
                : wait;
            return customWait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        /// Blocks until document.readyState === "complete".
        /// Helps avoid flakiness after navigation.
        /// </summary>
        public void WaitForPageLoad(int timeoutSeconds = 30)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds))
                .Until(drv => ((IJavaScriptExecutor)drv)
                .ExecuteScript("return document.readyState").Equals("complete"));
        }

        // ==============================
        // THEME STATE HELPERS
        // Used to assert Dark/Light mode changes (button text/class and effective bg color).
        // ==============================

        /// <summary>
        /// Ensures the theme toggle exists, is scrolled into view, and is clickable.
        /// </summary>
        private IWebElement WaitThemeButtonReady(int seconds = 10)
        {
            // Ensure presence in DOM
            var btn = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds))
                .Until(ExpectedConditions.ElementExists(BtnNavBarThemeDarkLight));

            // Bring into viewport center
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "arguments[0].scrollIntoView({block:'center', inline:'center'});", btn);

            // Ensure it is clickable
            return new WebDriverWait(driver, TimeSpan.FromSeconds(seconds))
                .Until(ExpectedConditions.ElementToBeClickable(BtnNavBarThemeDarkLight));
        }

        /// <summary>
        /// Snapshot of theme UI:
        /// - toggle button text,
        /// - toggle button CSS class,
        /// - effective background color at viewport center.
        /// </summary>
        public (string Text, string Class, string Bg) GetThemeUiState()
        {
            var btn = WaitThemeButtonReady();
            string text = (btn.Text ?? string.Empty).Trim();
            string cls = btn.GetAttribute("class") ?? string.Empty;
            string bg = UiHelpers.GetEffectiveBgColor(driver);
            return (text, cls, bg);
        }

        /// <summary>
        /// Toggles theme using the robust click approach (with JS fallback).
        /// </summary>
        public void ToggleTheme()
        {
            var btn = WaitThemeButtonReady();
            try
            {
                btn.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            }
        }

        /// <summary>
        /// Waits until the theme UI changes (text OR class OR background color).
        /// </summary>
        public void WaitThemeUiChanged((string Text, string Class, string Bg) before, int timeoutMs = 3000)
        {
            UiHelpers.WaitUntil(() =>
            {
                var now = GetThemeUiState();
                return now.Text != before.Text || now.Class != before.Class || now.Bg != before.Bg;
            }, timeoutMs);
        }

        /// <summary>
        /// Waits until the theme UI returns to the original snapshot.
        /// </summary>
        public void WaitThemeUiReverted((string Text, string Class, string Bg) original, int timeoutMs = 3000)
        {
            UiHelpers.WaitUntil(() =>
            {
                var now = GetThemeUiState();
                return now.Text == original.Text && now.Class == original.Class && now.Bg == original.Bg;
            }, timeoutMs);
        }

        // ==============================
        // PAGE WORKFLOWS
        // Small, page-specific flows used by tests.
        // ==============================

        public string GetTitle() => driver.Title;

        public void SubmitEmptyContactForm()
        {
            Click(BtnContactForm);
            Click(BtnContactFormSend);
        }

        public void SubmitInvalidContactForm()
        {
            Click(BtnContactForm);
            FillField(InputNameContactForm, "Name Test");
            FillField(InputEmailContactForm, "email-invalid");
            FillField(InputMessageContactForm, "Invalid test message.");
            Click(BtnContactFormSend);
        }

        public void SubmitValidContactForm()
        {
            Click(BtnContactForm);
            FillField(InputNameContactForm, "Name Test");
            FillField(InputEmailContactForm, "email.test@example.com.br");
            FillField(InputMessageContactForm, "Valid test message.");
            Click(BtnContactFormSend);
        }

        /// <summary>
        /// Selects a language by visible text in the navbar dropdown.
        /// </summary>
        public void SelectLanguage(string languageText)
        {
            Click(SelectorBtnNavBarLanguage);
            var languageSelectElement = wait.Until(
                ExpectedConditions.ElementIsVisible(SelectorBtnNavBarLanguage));
            var select = new SelectElement(languageSelectElement);
            select.SelectByText(languageText);
        }

        // ==============================
        // EXTERNAL LINK HELPERS
        // Used to validate outbound links and their final host after redirects.
        // ==============================

        /// <summary>
        /// Returns the trimmed href of the target element (waits for existence).
        /// </summary>
        public string GetHref(By locator)
        {
            var el = wait.Until(ExpectedConditions.ElementExists(locator));
            return (el.GetAttribute("href") ?? string.Empty).Trim();
        }

        /// <summary>
        /// Opens a URL in a new tab via JS if possible (fallback: same tab),
        /// waits for navigation, returns the final host, then cleans up the extra tab.
        /// </summary>
        public string OpenExternalAndGetFinalHost(string href, int timeoutMs = 7000)
        {
            var original = driver.CurrentWindowHandle;
            var before = driver.WindowHandles.Count;

            // Try to open in a new tab.
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open(arguments[0], '_blank');", href);

            // Wait for new handle; fallback to same-tab navigation if none appears.
            var end = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < end && driver.WindowHandles.Count == before)
                System.Threading.Thread.Sleep(100);

            if (driver.WindowHandles.Count > before)
            {
                var newHandle = driver.WindowHandles.First(h => h != original);
                driver.SwitchTo().Window(newHandle);
            }
            else
            {
                // Fallback: navigate current tab.
                driver.Navigate().GoToUrl(href);
            }

            // Final host (after any redirects).
            var finalHost = new Uri(driver.Url).Host.ToLowerInvariant();

            // If we opened an extra tab, close it and return to original.
            if (driver.WindowHandles.Count > 1)
            {
                driver.Close();
                driver.SwitchTo().Window(original);
            }

            return finalHost;
        }
    }
}
