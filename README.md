# 🧪 Automated Test Project — Ravi Professional Portfolio

This project contains **automated UI tests** for my personal portfolio website built with React.  
The main focus of this repository is **automated testing**, developed in **C# with Selenium WebDriver and NUnit**.

> ⚠️ The **React portfolio source code** is part of a **separate project**.  
> This repository includes **only the automation test suite** that validates the website’s key features, components, and UI behavior.

---

## 📋 Overview

The goal of this project is to demonstrate how to structure a **maintainable and readable automated test framework** using:
- The **Page Object Model (POM)** pattern,
- Explicit waits and helper utilities for UI synchronization,
- Reusable test flows and assertions,
- Multiple browser support (Chrome, Edge, Firefox),
- And a clear organization for scaling future test coverage.

This project also serves as a **learning and reference resource** for anyone looking to understand or build Selenium WebDriver projects with C# and NUnit.

---

## 🧠 Key Features

- Automated UI tests for the portfolio homepage (`https://ravifel.github.io/`)
- Page Object Model (POM) implementation
- Support for **Chrome**, **Edge**, and **Firefox**
- HTML5 validation checks for form fields
- Theme switch (Dark/Light) UI verification
- External link (LinkedIn, GitHub, WhatsApp) validation
- Centralized utility classes for waits, theme handling, and link verification
- Clean code with English comments for international readability

---

## 🧾 Example Test Scenarios

| **Test Case** | **Description** |
|----------------|----------------|
| `Home_Should_Open_And_Display_Expected_Title` | Verifies that the homepage loads correctly and the title contains “Ravi”. |
| `Navbar_Brand_Should_Redirect_To_Home` | Tests that clicking the site logo redirects back to the root page. |
| `Theme_Toggle_Should_Switch_Theme_And_Colors` | Ensures the dark/light mode toggle updates the UI correctly. |
| `Contact_Should_Show_Error_For_Invalid_Email` | Checks HTML5 validation messages for invalid email input. |
| `External_Links_Should_Open_Correct_Domain` | Validates that external links open the correct domains (LinkedIn, GitHub, WhatsApp). |

---

## 🧑‍💼 Author

**Ravi Silva**  
Software Quality Assurance Analyst  
📍 Dublin, Ireland  
📧 [ravifel.contact@gmail.com](mailto:ravifel.contact@gmail.com)

---

## 🌐 Profiles

🔗 [LinkedIn Profile](#)  
💻 [GitHub Portfolio](#)

---

## 🧱 License

This project is open for **educational and professional reference purposes**.  
You are free to fork, learn from, and adapt it for your own **Selenium WebDriver studies**.

---

## 💡 Notes

- This project is intentionally documented **entirely in English** to ensure global readability.  
- The test suite can serve both as a **learning resource** and a **real-world automation example** for portfolios or technical interviews.  
- The UI under test is hosted publicly at: [https://ravifel.github.io/](https://ravifel.github.io/)

---

## 🧩 Project Structure
<img width="412" height="496" alt="image" src="https://github.com/user-attachments/assets/42b9aad0-0c90-4359-9d99-1481c5240546" />

---

## ⚙️ Technologies & Tools

| Category | Technology |
|-----------|-------------|
| **Programming Language** | C# (.NET Framework 4.7.2) |
| **Test Framework** | NUnit 3 |
| **Automation Tool** | Selenium WebDriver |
| **Driver Management** | ChromeDriver, EdgeDriver, GeckoDriver |
| **IDE** | Visual Studio 2022 |
| **Build Tool** | .NET CLI / MSBuild |
| **Version Control** | Git & GitHub |

---

## 📦 Dependencies and Versions

Below is the complete list of NuGet packages used in this project:

| Package | Version | Description |
|----------|----------|-------------|
| DotNetSeleniumExtras.WaitHelpers | 3.11.0 | Provides explicit wait helpers for Selenium |
| Microsoft.CodeCoverage | 17.8.0 | Code coverage utilities |
| Microsoft.NET.Test.Sdk | 17.8.0 | Required for running tests via Visual Studio or CLI |
| NUnit | 3.14.0 | Core testing framework |
| NUnit.Analyzers | 3.9.0 | Static analysis for NUnit |
| NUnit3TestAdapter | 4.5.0 | Enables NUnit test discovery in Visual Studio |
| Selenium.Support | 4.8.0 | Selenium support libraries |
| Selenium.WebDriver | 4.8.0 | Core Selenium WebDriver bindings for C# |
| Selenium.WebDriver.ChromeDriver | 142.0.7444.6100 | ChromeDriver binary for Chrome 142.x |
| Selenium.WebDriver.GeckoDriver | 0.36.0 | GeckoDriver binary for Firefox |
| Selenium.WebDriver.IEDriver | 4.14.0 | IE WebDriver binary |

---

## 🚀 How to Run the Tests

### 🖥️ Prerequisites
- Windows, macOS, or Linux environment
- Visual Studio 2022 (or later)
- Google Chrome, Microsoft Edge, or Mozilla Firefox installed
- .NET Framework 4.7.2 SDK
- NuGet packages restored (`packages.config` dependencies)

### ⚡ Run from Visual Studio
1. Open the solution file:  
   `AutomatedTestProjectRaviWebsite.sln`
2. Build the project (`Ctrl + Shift + B`).
3. Open **Test Explorer** → Run All Tests.

### 🧑‍💻 Run from Command Line
```bash
# Example (run all tests)
dotnet test

# Run using a specific browser
set BROWSER=firefox
dotnet test
