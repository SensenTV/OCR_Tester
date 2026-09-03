using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR_Tester.Application;
using OCR_Tester.OCR.Tesseract;

namespace OCR_Tester.ConsoleUI
{
    // Stellt die Benutzerinteraktion für die Durchführung eines
    // OCR-Vergleichsdurchlaufs bereit.
    public class ComparisonMenu
    {
        ConsoleFormatter consoleFormatter = new ConsoleFormatter();
        ComparisonRunner comparisonRunner = new ComparisonRunner();

        public async Task StartComparisonAsync()
        {
            Console.Clear();
            consoleFormatter.PrintHeader("OCR-Vergleich");
            Console.WriteLine();

            await comparisonRunner.RunComparisonAsync();

            consoleFormatter.PrintInfo("Vergleich läuft...");
            consoleFormatter.PrintInfo("Vergleich abgeschlossen!");
            Console.WriteLine();
            consoleFormatter.PrintInfo(
                "Drücken Sie eine beliebige Taste, um zum Hauptmenü zurückzukehren."
            );
            Console.ReadKey();
        }
    }
}
