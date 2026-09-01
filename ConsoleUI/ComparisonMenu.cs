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
        public async Task StartComparisonAsync()
        {
            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine("       OCR Vergleich starten");
            Console.WriteLine("=================================");
            Console.WriteLine();
            // Hier können Sie die Logik für den Vergleich implementieren.
            // Zum Beispiel: Aufrufen der OCR-Engine, Vergleichen der Ergebnisse
            // mit der Ground Truth und Speichern der Ergebnisse.
            // Beispielhafte Ausgabe:
            Console.WriteLine("Vergleich läuft...");
            await Task.Delay(2000); // Simuliert eine Wartezeit für den Vergleich
            Console.WriteLine("Vergleich abgeschlossen!");
            Console.WriteLine();
            Console.WriteLine("Drücken Sie eine beliebige Taste, um zum Hauptmenü zurückzukehren.");
            Console.ReadKey();
        }
    }
}
