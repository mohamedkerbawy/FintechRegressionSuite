using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FintechRegressionSuite.Framework;

namespace FintechRegressionSuite.RiskModel
{
    public class BusinessFlow
    {
        public string FlowId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BusinessImpact { get; set; }
        public int ChangeFrequency { get; set; }
        public int HistoricalDefectDensity { get; set; }
        public RiskCategory RiskCategory { get; set; }
    }

    /// <summary>
    /// Loads the risk matrix and resolves which flows (and therefore which
    /// tagged tests) should run for a given pipeline stage.
    ///
    /// Usage in a pipeline:
    ///   PR build          -> GetFlowsAtOrAbove(RiskCategory.Critical)
    ///   Merge to main      -> GetFlowsAtOrAbove(RiskCategory.High)
    ///   Nightly / pre-release -> GetFlowsAtOrAbove(RiskCategory.Low)  // i.e. everything
    /// </summary>
    public class RiskBasedSelector
    {
        private readonly List<BusinessFlow> _flows;

        public RiskBasedSelector(string riskMatrixPath)
        {
            var json = System.IO.File.ReadAllText(riskMatrixPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("businessFlows");

            _flows = root.EnumerateArray().Select(el => new BusinessFlow
            {
                FlowId = el.GetProperty("flowId").GetString() ?? string.Empty,
                Description = el.GetProperty("description").GetString() ?? string.Empty,
                BusinessImpact = el.GetProperty("businessImpact").GetInt32(),
                ChangeFrequency = el.GetProperty("changeFrequency").GetInt32(),
                HistoricalDefectDensity = el.GetProperty("historicalDefectDensity").GetInt32(),
                RiskCategory = Enum.Parse<RiskCategory>(el.GetProperty("riskCategory").GetString()!)
            }).ToList();
        }

        public IEnumerable<BusinessFlow> GetFlowsAtOrAbove(RiskCategory minimum)
            => _flows.Where(f => f.RiskCategory >= minimum);

        /// <summary>
        /// Maps selected flows to the NUnit/Playwright test category filter string,
        /// e.g. "Critical|High", used directly as the --filter argument in the pipeline.
        /// </summary>
        public string ToCategoryFilter(RiskCategory minimum)
        {
            var categories = Enum.GetValues<RiskCategory>()
                .Cast<RiskCategory>()
                .Where(c => c >= minimum)
                .Select(c => c.ToString());

            return string.Join("|", categories);
        }
    }
}
