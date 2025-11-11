using AutomatedTestProjectRaviWebsite.Utils;
using AutomatedTestsRaviPortfolio.BasePage;
using AutomatedTestsRaviPortfolio.Pages;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AutomatedTestProjectRaviWebsite.Tests.Home
{
    /// <summary>
    /// UI tests for the public Home page.
    /// Uses the HomePage Page Object for clean and maintainable test logic.
    /// </summary>
    public class HomeTest : BaseClass
    {
        private HomePage homePage;

        /// <summary>
        /// Initializes the HomePage object and opens the site before each test.
        /// </summary>
        [SetUp]
        public void SetUpHome()
        {
            homePage = new HomePage(driver);
            homePage.Open();
        }

        /// <summary>
        /// Basic smoke test: verifies the page loads and the title contains "Ravi".
        /// </summary>
        [Test]
        public void Home_Should_Open_And_Display_Expected_Title()
        {
            var title = homePage.GetTitle();
            Assert.That(title, Is.Not.Null.And.Not.Empty);
            Assert.That(title, Does.Contain("Ravi").IgnoreCase);
        }

        /// <summary>
        /// Clicking the navbar brand (logo) should redirect to the site’s root.
        /// </summary>
        [Test]
        public void Navbar_Brand_Should_Redirect_To_Home()
        {
            // Navigate away first, then return via the brand button.
            homePage.Click(homePage.BtnNavBarRepositories);
            homePage.WaitForPageLoad();

            homePage.Click(homePage.BtnNavBarBrand);
            homePage.WaitForPageLoad();

            // GitHub Pages usually ends with "/" or "/index.html".
            Assert.That(driver.Url, Does.EndWith("/").Or.EndsWith("/index.html"));
            Assert.That(driver.Title, Does.Contain("Ravi").IgnoreCase);
        }

        /// <summary>
        /// "Repositories" navigation should change the URL accordingly.
        /// </summary>
        [Test]
        public void Navbar_Should_Navigate_To_Repositories()
        {
            homePage.Click(homePage.BtnNavBarRepositories);
            homePage.WaitForPageLoad();
            Assert.That(driver.Url.ToLowerInvariant(), Does.Contain("repo").Or.Contain("projects"));
        }

        /// <summary>
        /// "Testimonials" navigation should update the URL correctly.
        /// </summary>
        [Test]
        public void Navbar_Should_Navigate_To_Testimonials()
        {
            homePage.Click(homePage.BtnNavBarTestimonials);
            homePage.WaitForPageLoad();
            Assert.That(driver.Url.ToLowerInvariant(), Does.Contain("recom").Or.Contain("testi"));
        }

        /// <summary>
        /// Selecting English should set the <html lang="..."> attribute to an English locale.
        /// </summary>
        [Test]
        public void Language_Should_Switch_To_English_And_Set_HtmlLang()
        {
            homePage.SelectLanguage("English");
            homePage.WaitForPageLoad();

            var lang = ((IJavaScriptExecutor)driver).ExecuteScript("return document.documentElement.lang || ''")?.ToString();
            Assert.That(lang, Is.Not.Null.And.Not.Empty);
            Assert.That((lang ?? string.Empty).ToLowerInvariant(), Does.StartWith("en"));
        }

        /// <summary>
        /// Toggling the theme should update:
        /// - Button text (e.g., “Dark Mode” ↔ “Light Mode”)
        /// - Button class (Bootstrap style)
        /// - Background color.
        /// Toggling again should revert to the original state.
        /// </summary>
        [Test]
        public void Theme_Toggle_Should_Switch_Theme_And_Colors()
        {
            var before = homePage.GetThemeUiState();

            homePage.ToggleTheme();
            homePage.WaitThemeUiChanged(before);
            var after = homePage.GetThemeUiState();

            Assert.Multiple(() =>
            {
                Assert.That(after.Text, Is.Not.EqualTo(before.Text), "Button text did not change.");
                Assert.That(after.Class, Is.Not.EqualTo(before.Class), "Button class did not change.");
                Assert.That(after.Bg, Is.Not.EqualTo(before.Bg), "Background color did not change.");
            });

            homePage.ToggleTheme();
            homePage.WaitThemeUiReverted(before);
            var again = homePage.GetThemeUiState();

            Assert.Multiple(() =>
            {
                Assert.That(again.Text, Is.EqualTo(before.Text), "Button text did not revert.");
                Assert.That(again.Class, Is.EqualTo(before.Class), "Button class did not revert.");
                Assert.That(again.Bg, Is.EqualTo(before.Bg), "Background color did not revert.");
            });
        }

        /// <summary>
        /// Submitting an empty contact form should open the modal and trigger HTML5 validation errors.
        /// Name and Email fields should both be invalid.
        /// </summary>
        [Test]
        public void Contact_Should_Open_Modal_And_Validate_Empty_Form()
        {
            homePage.SubmitEmptyContactForm();
            var closeBtn = homePage.WaitForElementVisible(homePage.BtnContactFormClose, 10);
            Assert.That(closeBtn.Displayed, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(UiHelpers.IsInvalid(driver, homePage.InputNameContactForm), Is.True, "Name should be invalid.");
                Assert.That(UiHelpers.IsInvalid(driver, homePage.InputEmailContactForm), Is.True, "Email should be invalid.");
            });
        }

        /// <summary>
        /// Submitting an invalid email should trigger HTML5 validation (typeMismatch).
        /// </summary>
        [Test]
        public void Contact_Should_Show_Error_For_Invalid_Email()
        {
            homePage.SubmitInvalidContactForm();
            var (miss, type, msg) = UiHelpers.GetValidity(driver, homePage.InputEmailContactForm);
            Assert.That(type || miss, Is.True, $"Expected invalid email. validationMessage: '{msg}'");
        }

        /// <summary>
        /// Clicking the email contact button should display the Bootstrap modal.
        /// </summary>
        [Test]
        public void Contact_Email_Button_Should_Open_Modal()
        {
            homePage.Click(homePage.BtnContactForm);
            var modal = homePage.WaitForElementVisible(By.CssSelector("div.modal.show"), 10);
            Assert.That(modal.Displayed, Is.True);
        }

        /// <summary>
        /// Verifies that external links (LinkedIn, GitHub, WhatsApp):
        /// - Have an allowed href host.
        /// - Redirect to a final host also in the allowed list.
        /// </summary>
        [TestCase("btn-contact-linkedin")]
        [TestCase("btn-contact-github")]
        [TestCase("btn-contact-whatsapp")]
        public void External_Links_Should_Open_Correct_Domain(string elementId)
        {
            var allowed = LinkHelpers.AllowedHosts[elementId];

            LinkHelpers.AssertExternalLink(
                driver,
                By.Id(elementId),
                allowed,
                getHref: () => homePage.GetHref(By.Id(elementId)),
                openAndGetFinalHost: href => homePage.OpenExternalAndGetFinalHost(href)
            );
        }
    }
}
