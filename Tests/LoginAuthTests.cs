using FintechRegressionSuite.Framework;
using FintechRegressionSuite.PageObjects;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FintechRegressionSuite.Tests
{
    [TestFixture]
    [Category("Critical")] // AUTHENTICATION flow - see risk-matrix.json
    public class LoginAuthTests : BaseTest
    {
        [Test]
        public async Task ValidCredentials_WithMfa_LogsUserIn()
        {
            var login = new LoginPage(Page);
            await login.GotoAsync();
            await login.LoginAsync("qa.auto.user@example.com", "Test-Password-1!");
            await login.CompleteMfaAsync("123456");

            await Page.WaitForURLAsync("**/dashboard");
            Assert.That(Page.Url, Does.Contain("/dashboard"));
        }

        [Test]
        public async Task InvalidPassword_ShowsError_DoesNotAuthenticate()
        {
            var login = new LoginPage(Page);
            await login.GotoAsync();
            await login.LoginAsync("qa.auto.user@example.com", "WrongPassword!");

            var error = await login.GetErrorMessageAsync();
            Assert.That(error, Is.Not.Null.And.Contains("Invalid"));
        }

        [Test]
        public async Task SessionExpiry_RedirectsToLogin()
        {
            // Simulates an expired session cookie and asserts the guarded route
            // redirects back to /login rather than exposing account data.
            await Page.Context.AddCookiesAsync(new[]
            {
                new Microsoft.Playwright.Cookie
                {
                    Name = "session_token", Value = "expired-token", Url = TestConfig.BaseUrl
                }
            });

            await Page.GotoAsync("/dashboard");
            await Page.WaitForURLAsync("**/login");
            Assert.That(Page.Url, Does.Contain("/login"));
        }
    }
}
