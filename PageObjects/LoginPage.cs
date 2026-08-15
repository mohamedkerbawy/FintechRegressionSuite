using Microsoft.Playwright;
using System.Threading.Tasks;

namespace FintechRegressionSuite.PageObjects
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page) => _page = page;

        private ILocator UsernameInput => _page.GetByTestId("login-username");
        private ILocator PasswordInput => _page.GetByTestId("login-password");
        private ILocator SubmitButton => _page.GetByRole(AriaRole.Button, new() { Name = "Sign In" });
        private ILocator MfaCodeInput => _page.GetByTestId("mfa-code");
        private ILocator MfaSubmitButton => _page.GetByRole(AriaRole.Button, new() { Name = "Verify" });
        private ILocator ErrorBanner => _page.GetByTestId("login-error");

        public async Task GotoAsync() => await _page.GotoAsync("/login");

        public async Task LoginAsync(string username, string password)
        {
            await UsernameInput.FillAsync(username);
            await PasswordInput.FillAsync(password);
            await SubmitButton.ClickAsync();
        }

        public async Task CompleteMfaAsync(string code)
        {
            await MfaCodeInput.FillAsync(code);
            await MfaSubmitButton.ClickAsync();
        }

        public async Task<string?> GetErrorMessageAsync()
            => await ErrorBanner.IsVisibleAsync() ? await ErrorBanner.InnerTextAsync() : null;
    }
}
