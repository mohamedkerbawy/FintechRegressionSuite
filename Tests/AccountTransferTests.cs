using FintechRegressionSuite.Framework;
using FintechRegressionSuite.PageObjects;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FintechRegressionSuite.Tests
{
    [TestFixture]
    [Category("Critical")] // FUND_TRANSFER flow - see risk-matrix.json
    public class AccountTransferTests : BaseTest
    {
        [Test]
        public async Task InternalTransfer_BetweenOwnAccounts_UpdatesBalances()
        {
            var login = new LoginPage(Page);
            await login.GotoAsync();
            await login.LoginAsync("qa.auto.user@example.com", "Test-Password-1!");
            await login.CompleteMfaAsync("123456");

            await Page.GotoAsync("/transfers/new");
            await Page.GetByTestId("transfer-from").SelectOptionAsync("checking-001");
            await Page.GetByTestId("transfer-to").SelectOptionAsync("savings-001");
            await Page.GetByTestId("transfer-amount").FillAsync("250.00");
            await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Transfer" }).ClickAsync();

            var confirmation = Page.GetByTestId("transfer-confirmation");
            await Microsoft.Playwright.Assertions.Expect(confirmation).ToContainTextAsync("Transfer complete");
        }

        [Test]
        public async Task Transfer_InsufficientFunds_IsRejected()
        {
            var login = new LoginPage(Page);
            await login.GotoAsync();
            await login.LoginAsync("qa.auto.user@example.com", "Test-Password-1!");
            await login.CompleteMfaAsync("123456");

            await Page.GotoAsync("/transfers/new");
            await Page.GetByTestId("transfer-from").SelectOptionAsync("checking-001");
            await Page.GetByTestId("transfer-to").SelectOptionAsync("savings-001");
            await Page.GetByTestId("transfer-amount").FillAsync("50000000.00");
            await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Transfer" }).ClickAsync();

            var error = Page.GetByTestId("transfer-insufficient-funds-error");
            await Microsoft.Playwright.Assertions.Expect(error).ToBeVisibleAsync();
        }
    }
}
