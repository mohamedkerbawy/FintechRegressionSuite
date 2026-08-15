using FintechRegressionSuite.Framework;
using FintechRegressionSuite.PageObjects;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FintechRegressionSuite.Tests
{
    [TestFixture]
    [Category("Critical")] // PAYMENT_PROCESSING flow - see risk-matrix.json
    public class PaymentFlowTests : BaseTest
    {
        [Test]
        public async Task StandardPayment_CompletesSuccessfully()
        {
            var login = new LoginPage(Page);
            await login.GotoAsync();
            await login.LoginAsync("qa.auto.user@example.com", "Test-Password-1!");
            await login.CompleteMfaAsync("123456");

            var payment = new PaymentPage(Page);
            await payment.GotoAsync();
            await payment.FillPaymentAsync(payee: "payee-utility-co", amount: 125.50m, memo: "Regression test payment");
            await payment.ReviewAsync();

            var confirmation = await payment.ConfirmAsync();
            Assert.That(confirmation, Does.Contain("successful").IgnoreCase);
        }

        [Test]
        public async Task PaymentExceedingLimit_IsRejectedWithClearError()
        {
            var login = new LoginPage(Page);
            await login.GotoAsync();
            await login.LoginAsync("qa.auto.user@example.com", "Test-Password-1!");
            await login.CompleteMfaAsync("123456");

            var payment = new PaymentPage(Page);
            await payment.GotoAsync();
            await payment.FillPaymentAsync(payee: "payee-utility-co", amount: 999999.00m);
            await payment.ReviewAsync();

            var errorLocator = Page.GetByTestId("payment-limit-error");
            await Microsoft.Playwright.Assertions.Expect(errorLocator).ToBeVisibleAsync();
        }

        [Test]
        public async Task ZeroAmountPayment_IsBlockedClientSide()
        {
            var login = new LoginPage(Page);
            await login.GotoAsync();
            await login.LoginAsync("qa.auto.user@example.com", "Test-Password-1!");
            await login.CompleteMfaAsync("123456");

            var payment = new PaymentPage(Page);
            await payment.GotoAsync();
            await payment.FillPaymentAsync(payee: "payee-utility-co", amount: 0.00m);

            var reviewButton = Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Review Payment" });
            await Microsoft.Playwright.Assertions.Expect(reviewButton).ToBeDisabledAsync();
        }
    }
}
