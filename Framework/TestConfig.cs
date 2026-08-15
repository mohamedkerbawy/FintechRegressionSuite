using System;

namespace FintechRegressionSuite.Framework
{
    public static class TestConfig
    {
        // Pulled from pipeline variables / environment so the same suite runs
        // against Dev, Staging, or a pre-release environment.
        public static string BaseUrl =>
            Environment.GetEnvironmentVariable("TEST_BASE_URL") ?? "https://192.168.102.90:9443/anb_retail_sp2";

        public static string RiskMatrixPath =>
            Environment.GetEnvironmentVariable("RISK_MATRIX_PATH") ?? "RiskModel/risk-matrix.json";
    }
}
