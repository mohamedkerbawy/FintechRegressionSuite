using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FintechRegressionSuite.Framework
{
    [TestFixture]
    public abstract class BaseTest
    {
        protected IPlaywright PlaywrightInstance = null!;
        protected IBrowser Browser = null!;
        protected IPage Page = null!;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            PlaywrightInstance = await Playwright.CreateAsync();
            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
        }

        [SetUp]
        public async Task TestSetup()
        {
            var context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                BaseURL = TestConfig.BaseUrl,
                IgnoreHTTPSErrors = true
            });
            Page = await context.NewPageAsync();
        }

        [TearDown]
        public async Task TestTeardown()
        {
            // Attach a screenshot on failure for faster triage in Azure DevOps.
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var path = $"artifacts/{TestContext.CurrentContext.Test.Name}.png";
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
                TestContext.AddTestAttachment(path);
            }

            await Page.Context.CloseAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await Browser.DisposeAsync();
            PlaywrightInstance.Dispose();
        }
    }
}
