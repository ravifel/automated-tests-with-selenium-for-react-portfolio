using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace AutomatedTestProjectRaviWebsite.Utils
{
    /// <summary>
    /// Utilities for validating external links (social/contact buttons, etc.).
    /// Centralizes allowed hostnames and assertions used in link verification tests.
    /// </summary>
    public static class LinkHelpers
    {
        /// <summary>
        /// Maps element IDs to allowed hostnames.
        /// Extend this dictionary when new external buttons are added.
        /// </summary>
        public static readonly Dictionary<string, string[]> AllowedHosts = new Dictionary<string, string[]>
        {
            { "btn-contact-linkedin", new[] { "linkedin.com", "www.linkedin.com" } },
            { "btn-contact-github",   new[] { "github.com", "www.github.com" } },
            { "btn-contact-whatsapp", new[] { "wa.me", "api.whatsapp.com" } }
        };

        /// <summary>
        /// Validates an external link by checking:
        /// 1) The element’s href host is allowed.
        /// 2) The final host after redirects is also allowed.
        ///
        /// The test injects two delegates:
        /// - getHref: retrieves the href from the page object.
        /// - openAndGetFinalHost: opens the link and returns the resolved host.
        /// </summary>
        public static void AssertExternalLink(
            IWebDriver driver,
            By locator,
            string[] allowedHosts,
            Func<string> getHref,
            Func<string, string> openAndGetFinalHost)
        {
            // 1) Validate href host
            var href = getHref();
            NUnit.Framework.Assert.That(href, NUnit.Framework.Is.Not.Empty, "Element has no href.");
            var hrefHost = new Uri(href).Host.ToLowerInvariant();
            NUnit.Framework.Assert.That(
                allowedHosts.Any(h => hrefHost.Contains(h)),
                NUnit.Framework.Is.True,
                $"Href host '{hrefHost}' not in allowed [{string.Join(", ", allowedHosts)}]."
            );

            // 2) Validate final redirected host
            var finalHost = openAndGetFinalHost(href);
            NUnit.Framework.Assert.That(
                allowedHosts.Any(h => finalHost.Contains(h)),
                NUnit.Framework.Is.True,
                $"Final host '{finalHost}' not in allowed [{string.Join(", ", allowedHosts)}]."
            );
        }
    }
}
