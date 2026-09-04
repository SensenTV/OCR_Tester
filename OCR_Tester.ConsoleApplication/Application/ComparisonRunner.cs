using Microsoft.Extensions.Configuration;
using OCR_Tester.Configuration;
using OCR_Tester.Evaluation;
using OCR_Tester.Input;
using OCR_Tester.OCR.GLM;
using OCR_Tester.OCR.Tesseract;

namespace OCR_Tester.Application
{
    /// <summary>
    /// Steuert den vollständigen Ablauf eines OCR-Vergleichsdurchlaufs.
    /// Koordiniert das Einlesen der Testdaten, die OCR-Verarbeitung,
    /// die Auswertung sowie die Speicherung der Ergebnisse.
    /// </summary>
    public class ComparisonRunner
    {
        /// <summary>
        /// Führt den vollständigen Ablauf eines OCR-Vergleichsdurchlaufs asynchron aus.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task RunComparisonAsync()
        {
            // Schritt 1: Testdaten einlesen
            DataLoader dataLoader = new DataLoader();
            var testCases = dataLoader.LoadImageTestCases();

            Console.WriteLine($"Es wurden {testCases.Count} Testfälle geladen.");

            // Schritt 2: OCR-Verarbeitung durchführen

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Configuration/appsettings.json", optional: false)
                .Build();

            var settings =
                configuration.Get<AppSettings>()
                ?? throw new InvalidOperationException(
                    "Die AppSettings konnten nicht geladen werden."
                );

            GlmOcrEngine glmOcrEngine = new GlmOcrEngine(
                settings.GlmOcr.Endpoint,
                settings.GlmOcr.ApiKey,
                settings.GlmOcr.Model,
                settings.GlmOcr.Prompt
            );
            TesseractOcrEngine tesseractOcrEngine = new TesseractOcrEngine(
                settings.Tesseract.TessDataPath,
                settings.Tesseract.Language,
                settings.Tesseract.EngineMode,
                settings.Tesseract.PageSegmentationMode
            );

            foreach (var testCase in testCases)
            {
                var glmOcrResult = await glmOcrEngine.ProcessImageAsync(testCase);
                var tesseractOcrResult = await tesseractOcrEngine.ProcessImageAsync(testCase);

                // Hier können Sie die Ergebnisse vergleichen und auswerten
                Console.WriteLine($"Bild: {testCase.ImageName}");
                Console.WriteLine($"GLM-OCR Ergebnis: {glmOcrResult.RecognizedText}");
                Console.WriteLine($"Tesseract Ergebnis: {tesseractOcrResult.RecognizedText}");
                Console.WriteLine();

                // Schritt 3: Ergebnisse auswerten
                CerCalculator cerCalculator = new CerCalculator();
                var glmBenchmark = cerCalculator.Calculate(
                    testCase.GroundTruth.ExpectedText,
                    glmOcrResult.RecognizedText
                );
                var tesseractBenchmark = cerCalculator.Calculate(
                    testCase.GroundTruth.ExpectedText,
                    tesseractOcrResult.RecognizedText
                );
                Console.WriteLine(
                    $"""
                    {glmOcrResult.ModelName} CER: {glmBenchmark.CharacterErrorRate} / {glmBenchmark.CharacterErrorRateInPercent}%
                    {tesseractOcrResult.ModelName} CER: {tesseractBenchmark.CharacterErrorRate} / {tesseractBenchmark.CharacterErrorRateInPercent}%
                    """
                );

                // Schritt 4: Ergebnisse speichern
                //SaveResults(evaluationResults);
            }
        }
    }
}
