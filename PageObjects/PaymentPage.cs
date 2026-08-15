using Microsoft.Playwright;
using System.Threading.Tasks;

namespace FintechRegressionSuite.PageObjects
{
    public class PaymentPage
    {
        private readonly IPage _page;

        public PaymentPage(IPage page) => _page = page;

        private ILocator PayeeSelect => _page.GetByTestId("payment-payee");
        private ILocator AmountInput => _page.GetByTestId("payment-amount");
        private ILocator MemoInput => _page.GetByTestId("payment-memo");
        private ILocator ReviewButton => _page.GetByRole(AriaRole.Button, new() { Name = "Review Payment" });
        private ILocator ConfirmButton => _page.GetByRole(AriaRole.Button, new() { Name = "Confirm Payment" });
        private ILocator ConfirmationBanner => _page.GetByTestId("payment-confirmation");

        public async Task GotoAsync() => await _page.GotoAsync("/payments/new");

        public async Task FillPaymentAsync(string payee, decimal amount, string memo = "")
        {
            await PayeeSelect.SelectOptionAsync(payee);
            await AmountInput.FillAsync(amount.ToString("0.00"));
            if (!string.IsNullOrEmpty(memo))
                await MemoInput.FillAsync(memo);
        }

        public async Task ReviewAsync() => await ReviewButton.ClickAsync();

        public async Task<string> ConfirmAsync()
        {
            await ConfirmButton.ClickAsync();
            await ConfirmationBanner.WaitForAsync();
            return await ConfirmationBanner.InnerTextAsync();
        }
    }
}
