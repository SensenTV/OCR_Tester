using Spectre.Console;
using Serilog;

namespace OCR_Tester.ConsoleUI
{
    public class MainMenu
    {
        private readonly ConsoleFormatter _consoleFormatter = new();

        public async Task ShowAsync()
        {
            bool running = true;

            while (running)
            {
                AnsiConsole.Clear();

                _consoleFormatter.PrintHeader(
                    "OCR Benchmark - Hauptmenü");

                string selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[cyan]Was möchtest du tun?[/]")
                        .AddChoices(
                            "Vergleich starten",
                            "Beenden"));

                switch (selection)
                {
                    case "Vergleich starten":
                        Log.Information(
                            "Starting OCR benchmark.");

                        var comparisonMenu = new ComparisonMenu();

                        await comparisonMenu.StartComparisonAsync();
                        break;

                    case "Beenden":
                        Log.Information(
                            "Exiting the application.");

                        running = false;
                        break;
                }
            }
        }
    }
}