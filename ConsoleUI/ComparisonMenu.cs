using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Tester.ConsoleUI
{
    // Stellt die Benutzerinteraktion für die Durchführung eines
    // OCR-Vergleichsdurchlaufs bereit.
    public   class ComparisonMenu
    {
        ConsoleFormatter consoleFormatter = new ConsoleFormatter();

        public async Task StartComparisonAsync()
        {
            Console.Clear();
            consoleFormatter.PrintHeader("OCR-Vergleich");
            Console.WriteLine();
            // Hier können Sie die Logik für den Vergleich implementieren.
            // Zum Beispiel: Aufrufen der OCR-Engine, Vergleichen der Ergebnisse
            // mit der Ground Truth und Speichern der Ergebnisse.
            // Beispielhafte Ausgabe:
            consoleFormatter.PrintInfo("Vergleich läuft...");
            consoleFormatter.PrintInfo("Vergleich abgeschlossen!");
            Console.WriteLine();
            consoleFormatter.PrintInfo("Drücken Sie eine beliebige Taste, um zum Hauptmenü zurückzukehren.");
            Console.ReadKey();
        }
    }
}
