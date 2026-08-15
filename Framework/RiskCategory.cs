namespace FintechRegressionSuite.Framework
{
    /// <summary>
    /// Risk tier assigned to a business flow / test, derived from the risk matrix
    /// (business impact x change frequency x historical defect density).
    /// </summary>
    public enum RiskCategory
    {
        Critical = 4, // Money movement, auth - must pass on every PR/merge
        High = 3,     // Core account management - runs on merge + nightly
        Medium = 2,   // Secondary flows - runs nightly / pre-release
        Low = 1       // Rarely-changed, low-impact screens - runs pre-release only
    }
}
