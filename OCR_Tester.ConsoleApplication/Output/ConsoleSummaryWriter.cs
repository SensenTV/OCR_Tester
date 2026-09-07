using System.Text;
using OCR_Tester.Domain.Models;
using Serilog;

namespace OCR_Tester.Output
{
    public class ConsoleSummaryWriter
    {
        public string Format(IEnumerable<BenchmarkModelSummary> summaries)
        {
            var summaryBuilder = new StringBuilder();

            foreach (var modelSummary in summaries)
            {
                AppendModelSummary(summaryBuilder, modelSummary);
            }

            Log.Information("Formatted benchmark summary for console output.");

            return summaryBuilder.ToString();
        }

        private void AppendModelSummary(StringBuilder builder, BenchmarkModelSummary modelSummary)
        {
            builder.AppendLine($"Model: {modelSummary.ModelName}");
            builder.AppendLine($"Total Images Processed: {modelSummary.TotalImagesProcessed}");
            builder.AppendLine($"Total Processing Time (ms): {modelSummary.TotalProcessingTimeMs}");
            builder.AppendLine(
                $"Average Processing Time (ms): {modelSummary.AverageProcessingTimeMs}"
            );
            builder.AppendLine(
                $"Average Character Error Rate (%): {modelSummary.OverallCharacterErrorRate:F2}"
            );
            builder.AppendLine(
                $"Overall Character Error Rate: {modelSummary.OverallCharacterErrorRate}"
            );
            builder.AppendLine($"Total Input Tokens: {modelSummary.TotalInputTokens}");
            builder.AppendLine($"Total Output Tokens: {modelSummary.TotalOutputTokens}");
            builder.AppendLine();
        }
    }
}
