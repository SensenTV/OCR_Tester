using OCR_Tester.Application;
using Serilog;
using Spectre.Console;

namespace OCR_Tester.ConsoleUI
{
    public class ComparisonMenu
    {
        private readonly ConsoleFormatter _consoleFormatter = new();
        private readonly ComparisonRunner _comparisonRunner = new();

        public async Task StartComparisonAsync()
        {
            AnsiConsole.Clear();

            _consoleFormatter.PrintHeader("OCR Benchmark - Vergleich");

            Log.Information("Starting OCR comparison.");
            await _comparisonRunner.RunComparisonAsync();

            _consoleFormatter.WaitForKey();
        }
    }
}
