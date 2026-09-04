using Microsoft.Extensions.Configuration;
using OCR_Tester.Configuration;
using OCR_Tester.ConsoleUI;
using OCR_Tester.Domain.Interfaces;
using OCR_Tester.Domain.Models;
using OCR_Tester.Evaluation;
using OCR_Tester.Input;
using OCR_Tester.Output;
using Spectre.Console;

namespace OCR_Tester.Application
{
    /// <summary>
    /// Steuert den vollständigen Ablauf eines OCR-Vergleichsdurchlaufs.
    /// Koordiniert das Einlesen der Testdaten, die OCR-Verarbeitung,
    /// die Auswertung sowie die Speicherung der Ergebnisse.
    /// </summary>
    public class ComparisonRunner
    {
        private readonly ConsoleFormatter _consoleFormatter = new();

        /// <summary>
        /// Führt den vollständigen OCR-Benchmark asynchron aus.
        /// </summary>
        public async Task RunComparisonAsync()
        {
            // ============================================================
            // Ergebnisordner vorbereiten
            // ============================================================

            string resultsDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "Results",
                DateTime.Now.ToString("yyyyMMdd_HHmmss")
            );

            Directory.CreateDirectory(resultsDirectory);

            // ============================================================
            // Komponenten vorbereiten
            // ============================================================

            var jsonResultWriter = new JsonResultWriter();
            var cerCalculator = new CerCalculator();
            var summaryCalculator = new BenchmarkSummaryCalculator();

            // ============================================================
            // Testdaten laden
            // ============================================================

            var dataLoader = new DataLoader();
            var testCases = dataLoader.LoadImageTestCases();

            if (testCases.Count == 0)
            {
                _consoleFormatter.PrintError("Es wurden keine Testfälle gefunden.");

                return;
            }

            // ============================================================
            // Konfiguration laden
            // ============================================================

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Configuration/appsettings.json", optional: false)
                .Build();

            var settings =
                configuration.Get<AppSettings>()
                ?? throw new InvalidOperationException(
                    "Die AppSettings konnten nicht geladen werden."
                );

            // ============================================================
            // OCR-Engines erstellen
            // ============================================================

            var factory = new OcrEngineFactory();

            List<IOcrEngine> ocrEngines = settings.OcrEngines.Select(factory.Create).ToList();

            if (ocrEngines.Count == 0)
            {
                _consoleFormatter.PrintError("Es wurden keine OCR-Engines konfiguriert.");

                return;
            }

            // ============================================================
            // Benchmark-Informationen anzeigen
            // ============================================================

            _consoleFormatter.PrintBenchmarkStart(testCases.Count, ocrEngines.Count);

            // ============================================================
            // OCR-Verarbeitung
            // ============================================================

            int totalOperations = testCases.Count * ocrEngines.Count;

            int currentOperation = 0;

            await AnsiConsole
                .Progress()
                .AutoClear(false)
                .Columns(
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn()
                )
                .StartAsync(async ctx =>
                {
                    var progressTask = ctx.AddTask(
                        "[cyan]OCR Benchmark[/]",
                        maxValue: totalOperations
                    );

                    foreach (var testCase in testCases)
                    {
                        foreach (var engine in ocrEngines)
                        {
                            progressTask.Description =
                                $"[cyan]{Markup.Escape(engine.ModelName)}[/] "
                                + $"[grey]→ {Markup.Escape(testCase.ImageName)}[/]";

                            // OCR durchführen
                            var result = await engine.ProcessImageAsync(testCase);

                            // CER berechnen
                            result.SingleBenchmark = cerCalculator.Calculate(
                                testCase.ExpectedText,
                                result.RecognizedText
                            );

                            // Ergebnis zur Summary hinzufügen
                            summaryCalculator.AddResult(result);

                            // Einzelnes Ergebnis speichern
                            string resultFilePath = Path.Combine(
                                resultsDirectory,
                                $"{testCase.ImageName}_{result.ModelName}_Result.json"
                            );

                            jsonResultWriter.WriteResultsToJsonFile(resultFilePath, result);

                            // Fortschritt aktualisieren
                            currentOperation++;

                            progressTask.Value = currentOperation;
                        }
                    }
                });

            // ============================================================
            // Gesamtergebnis
            // ============================================================

            BenchmarkSummary summary = summaryCalculator.Summary;

            // ============================================================
            // Summary speichern
            // ============================================================

            jsonResultWriter.WriteResultsToJsonFile(
                Path.Combine(resultsDirectory, "SummaryResults.json"),
                summary
            );

            // ============================================================
            // Summary anzeigen
            // ============================================================

            Console.WriteLine();

            _consoleFormatter.PrintSummary(summary.Models);

            // ============================================================
            // Abschlussmeldung
            // ============================================================

            Console.WriteLine();

            _consoleFormatter.PrintBenchmarkFinished(resultsDirectory);
        }
    }
}
