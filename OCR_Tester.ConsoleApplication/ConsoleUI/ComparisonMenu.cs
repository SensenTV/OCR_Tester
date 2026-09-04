using Spectre.Console;
using OCR_Tester.Application;

namespace OCR_Tester.ConsoleUI
{
    public class ComparisonMenu
    {
        private readonly ConsoleFormatter _consoleFormatter = new();
        private readonly ComparisonRunner _comparisonRunner = new();

        public async Task StartComparisonAsync()
        {
            AnsiConsole.Clear();

            _consoleFormatter.PrintHeader("OCR-Vergleich");

            await _comparisonRunner.RunComparisonAsync();

            _consoleFormatter.WaitForKey();
        }
    }
}