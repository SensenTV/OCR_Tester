using Microsoft.Extensions.Configuration;
using OCR_Tester.Configuration;
using OCR_Tester.ConsoleUI;
using OCR_Tester.Domain.Interfaces;
using OCR_Tester.Domain.Models;
using OCR_Tester.Evaluation;
using OCR_Tester.Input;
using OCR_Tester.Output;
using Serilog;
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

            AppSettings? settings = null;
            var jsonResultWriter = new JsonResultWriter();
            var cerCalculator = new CerCalculator();
            var apiKey = Environment.GetEnvironmentVariable(
                "GLM_API_KEY",
                EnvironmentVariableTarget.User
            );
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Log.Error("API key not found. Exiting the comparison runner.");
                _consoleFormatter.PrintError(
                    "Die Umgebungsvariable 'GLM_API_KEY' ist nicht gesetzt. Setze sie bitte (Siehe README.md für Details) und starte die Anwendung erneut."
                );

                throw new InvalidOperationException(
                    "Die Umgebungsvariable 'GLM_API_KEY' ist nicht gesetzt.Setze sie bitte(Siehe README.md für Details) und starte die Anwendung erneut. "
                );
            }

            // ============================================================
            // Testdaten laden
            // ============================================================

            var dataLoader = new DataLoader();
            var testCases = dataLoader.LoadImageTestCases();

            if (testCases.Count == 0)
            {
                _consoleFormatter.PrintError(
                    "Es wurden keine Testfälle gefunden. Überprüfe ob der angegebene Ordner BMP-Bilder und die zugehörigen Ground-Truth-JSON-Dateien enthält."
                );
                Log.Warning("No test cases found. Exiting the comparison runner.");
                return;
            }

            // ============================================================
            // Konfiguration laden
            // ============================================================

            try
            {
                Log.Information("Loading configuration from appsettings.json...");
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("Configuration/appsettings.json", optional: false)
                    .Build();

                settings = configuration.Get<AppSettings>();
                if (settings == null)
                {
                    Log.Error("Failed to load configuration. Exiting the comparison runner.");
                    _consoleFormatter.PrintError(
                        "Fehler beim Laden der Konfiguration. Die appsettings.json-Datei scheint leer zu sein. Bitte überprüfen Sie die Datei."
                    );
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while loading configuration.");
                _consoleFormatter.PrintError(
                    "Fehler beim Laden der Konfiguration. Die appsettings.json-Datei scheint nicht korrekt zu sein. Bitte überprüfen Sie die Datei."
                );
                return;
            }

            // ============================================================
            // API-Key für GLM-Engine setzen (In meinem Fall: Für Unsloth studio auf dem das Modell läuft)
            // ============================================================
            settings
                .OcrEngines.First(x => x.Type.Equals("glm", StringComparison.OrdinalIgnoreCase))
                .ApiKey = apiKey;

            // ============================================================
            // BenchmarkSummaryCalculator initialisieren
            // ============================================================
            var summaryCalculator = new BenchmarkSummaryCalculator(settings);

            // ============================================================
            // OCR-Engines erstellen
            // ============================================================

            var factory = new OcrEngineFactory();

            List<IOcrEngine> ocrEngines = settings.OcrEngines.Select(factory.Create).ToList();

            if (ocrEngines.Count == 0)
            {
                _consoleFormatter.PrintError("Es wurden keine OCR-Engines konfiguriert.");
                Log.Warning("No OCR engines configured. Exiting the comparison runner.");

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
                            Log.Information(
                                "Saved individual result for image '{ImageName}' and model '{ModelName}' to '{ResultFilePath}'.",
                                testCase.ImageName,
                                result.ModelName,
                                resultFilePath
                            );

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
            Log.Information(
                "Saved benchmark summary to '{SummaryFilePath}'.",
                Path.Combine(resultsDirectory, "SummaryResults.json")
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
