using Serilog;
using System.Threading.Tasks;
using System;

namespace OCR_Tester.ConsoleUI
{
    // Stellt das Hauptmenü der Konsolenanwendung dar und verarbeitet
    // die Auswahl des Benutzers.
    public class MainMenu
    {
        public async Task ShowAsync()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();

                Log.Information("Displaying the main menu.");

                Console.WriteLine("=================================");
                Console.WriteLine("          OCR Tester");
                Console.WriteLine("=================================");
                Console.WriteLine();
                Console.WriteLine("1. Vergleich starten");
                Console.WriteLine("2. Beenden");
                Console.WriteLine();
                Console.Write("Auswahl: ");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        // Start the OCR Benchmark
                        var _ComparisonMenu = new ComparisonMenu();
                        await _ComparisonMenu.StartComparisonAsync();
                        break;
                    case "2":
                        // Exit the application
                        Log.Information("Exiting the application.");
                        Environment.Exit(0);
                        break;
                    default:
                        Log.Warning("Invalid menu option selected.");
                        Console.WriteLine("Invalid option. Please try again.");
                        break;

                }
            }

        }
    }
}