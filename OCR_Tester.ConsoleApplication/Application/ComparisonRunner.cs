using Microsoft.Extensions.Configuration;
using OCR_Tester.Configuration;
using OCR_Tester.Input;
using OCR_Tester.OCR.GLM;
using OCR_Tester.OCR.Tesseract;

namespace OCR_Tester.Application
{
    // Steuert den vollständigen Ablauf eines OCR-Vergleichsdurchlaufs.
    // Koordiniert das Einlesen der Testdaten, die OCR-Verarbeitung,
    // die Auswertung sowie die Speicherung der Ergebnisse.
    public class ComparisonRunner
    {
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
            }

            // Schritt 3: Ergebnisse auswerten
            //var evaluationResults = EvaluateResults(testCases, ocrResults);

            // Schritt 4: Ergebnisse speichern
            //SaveResults(evaluationResults);
        }
    }
}
