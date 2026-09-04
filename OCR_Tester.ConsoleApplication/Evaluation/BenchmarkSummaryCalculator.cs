using OCR_Tester.Domain.Models;

namespace OCR_Tester.Evaluation
{
    public class BenchmarkSummaryCalculator
    {
        private readonly BenchmarkSummary _summary = new();

        public BenchmarkSummary Summary => _summary;

        public void AddResult(OcrResult result)
        {
            var modelSummary = GetOrCreateModelSummary(result.ModelName);

            modelSummary.TotalImagesProcessed++;

            modelSummary.TotalProcessingTimeMs += result.ProcessingTimeMs;

            modelSummary.OverallCharacterErrorRate =
                (
                    (
                        modelSummary.OverallCharacterErrorRate
                        * (modelSummary.TotalImagesProcessed - 1)
                    ) + result.CharacterErrorRateInPercent
                ) / modelSummary.TotalImagesProcessed;

            modelSummary.OverallCharacterErrors += result.CharacterErrorRate;

            modelSummary.TotalInputTokens += result.InputTokens;

            modelSummary.TotalOutputTokens += result.OutputTokens;

            modelSummary.AverageProcessingTimeMs =
                modelSummary.TotalProcessingTimeMs / modelSummary.TotalImagesProcessed;
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

            return modelSummary;
        }
    }
}
