using Microsoft.Extensions.Configuration;
using OCR_Tester.Configuration;
using OCR_Tester.Domain.Models;
using Serilog;

namespace OCR_Tester.Evaluation
{
    public class BenchmarkSummaryCalculator
    {
        private readonly BenchmarkSummary _summary = new();
        private readonly AppSettings _settings;

        public BenchmarkSummary Summary => _summary;

        public BenchmarkSummaryCalculator(AppSettings settings)
        {
            _settings = settings;

            Log.Information(
                "BenchmarkSummaryCalculator initialized with settings: {@Settings}",
                _settings
            );
        }

        public void AddResult(OcrResult result)
        {
            var modelSummary = GetOrCreateModelSummary(result.ModelName);

            modelSummary.TotalImagesProcessed++;

            modelSummary.TotalProcessingTimeMs += result.ProcessingTimeMs;

            modelSummary.OverallCharacterErrorRate = Math.Round(
                (
                    (
                        modelSummary.OverallCharacterErrorRate
                        * (modelSummary.TotalImagesProcessed - 1)
                    ) + result.CharacterErrorRateInPercent
                ) / modelSummary.TotalImagesProcessed,
                2
            );

            modelSummary.OverallCharacterErrors += result.CharacterErrorRate;

            modelSummary.TotalInputTokens += result.InputTokens;

            modelSummary.TotalOutputTokens += result.OutputTokens;

            modelSummary.AverageProcessingTimeMs =
                modelSummary.TotalProcessingTimeMs / modelSummary.TotalImagesProcessed;

            var engineSettings = _settings.OcrEngines.FirstOrDefault(x =>
                x.Name.Equals(result.ModelName, StringComparison.OrdinalIgnoreCase)
            );

            if (engineSettings != null)
            {
                modelSummary.GraphicsProcessingUnit = engineSettings.GraphicsProcessingUnit;
                modelSummary.VRAM = engineSettings.VRAM;
                modelSummary.CPU = engineSettings.CPU;
                modelSummary.RAM = engineSettings.RAM;
            }

            Log.Information("Updated summary for model {ModelName}", result.ModelName);
        }

        private BenchmarkModelSummary GetOrCreateModelSummary(string modelName)
        {
            var modelSummary = _summary.Models.FirstOrDefault(x =>
                x.ModelName.Equals(modelName, StringComparison.OrdinalIgnoreCase)
            );

            if (modelSummary != null)
            {
                return modelSummary;
            }

            modelSummary = new BenchmarkModelSummary { ModelName = modelName };

            _summary.Models.Add(modelSummary);

            Log.Information("Created new summary for model {ModelName}", modelName);

            return modelSummary;
        }
    }
}
