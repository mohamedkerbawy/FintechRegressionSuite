# Fintech Regression Suite — Risk-Based Test Automation Framework

Sample framework Demonstrates risk-based regression analysis, C#/Playwright automation, and Azure DevOps
CI/CD integration for a fintech platform's critical business flows.

## Structure

```
FintechRegressionSuite/
├── Framework/
│   ├── RiskCategory.cs        # Critical / High / Medium / Low enum
│   ├── BaseTest.cs            # Playwright lifecycle, Setup, Teardown, screenshot-on-failure
│   └── TestConfig.cs          # Environment/base URL resolution, Risk matrix path
├── RiskModel/
│   ├── risk-matrix.json       # Business flows scored by impact/frequency/defect history
│   └── RiskBasedSelector.cs   # Resolves which risk tier(s) to run per pipeline stage
├── PageObjects/
│   ├── LoginPage.cs
│   └── PaymentPage.cs
├── Tests/
│   ├── LoginAuthTests.cs      
│   ├── PaymentFlowTests.cs    
│   └── AccountTransferTests.cs 
├── pipelines/
│   └── azure-pipelines.yml    # Tiered execution: PR -> Critical, merge -> Critical+High, nightly -> Full
└── docs/
    └── case-study-fintech-release-risk.md
```

## How risk-based selection works

1. **`risk-matrix.json`** is the source of truth for which business flows are Critical/High/Medium/Low,
   scored on business impact, change frequency, and historical defect density. It's reviewed by QE and
   Product each release cycle, not auto-generated — the raw scores inform the category, a human confirms it.
2. Each test class is tagged with `[Category("Critical")]` (etc.), matching a flow in the matrix.
3. `RiskBasedSelector` (or the pipeline's inline filter logic) resolves which categories should run for
   a given trigger: PR, merge, or nightly/pre-release.
4. The Azure DevOps pipeline passes that as an NUnit `--filter` to `dotnet test`, so the same test project
   serves fast PR feedback and full pre-release regression without duplicating test code.

## Running locally

```bash
dotnet restore
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps chromium

# Critical tier only
dotnet test --filter "TestCategory=Critical"

# Full suite
dotnet test
```

## Extending

- Add a new business flow: append it to `risk-matrix.json`, create the Page Object, write the test,
  tag it with the matching `[Category(...)]`.
- Re-scoring a flow's risk (e.g. after a defect in production) is a one-line change in the JSON —
  no test code changes required to move it into the PR-blocking tier.
