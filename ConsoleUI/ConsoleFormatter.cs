using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Tester.ConsoleUI
{
    /// <summary>
    /// Verantwortlich für die formatierte Darstellung von Informationen
    /// in der Konsolenanwendung.
    /// 
    /// Die Klasse enthält keine Programmlogik und trifft keine Entscheidungen.
    /// Sie kümmert sich ausschließlich um die Ausgabe und Formatierung.
    /// </summary>
    public class ConsoleFormatter
    {
        const int width = 40;

        /// <summary>
        /// Gibt eine Überschrift mit Trennlinien aus.
        /// </summary>
        /// <param name="title">Titel der aktuellen Ansicht.</param>
        public void PrintHeader(string title)
        {

            Console.WriteLine(new string('=', width));
            Console.WriteLine(title.PadLeft((width + title.Length) / 2));
            Console.WriteLine(new string('=', width));
            Console.WriteLine();
        }

        /// <summary>
        /// Gibt eine Erfolgsmeldung aus.
        /// </summary>
        /// <param name="message">Anzuzeigende Nachricht.</param>
        public void PrintSuccess(string message)
        {
            Console.WriteLine($"[OK] {message}");
        }

        /// <summary>
        /// Gibt eine Fehlermeldung aus.
        /// </summary>
        /// <param name="message">Anzuzeigende Fehlermeldung.</param>
        public void PrintError(string message)
        {
            Console.WriteLine($"[FEHLER] {message}");
        }

        /// <summary>
        /// Gibt eine Warnung aus.
        /// </summary>
        /// <param name="message">Anzuzeigende Warnung.</param>
        public void PrintWarning(string message)
        {
            Console.WriteLine($"[WARNUNG] {message}");
        }

        /// <summary>
        /// Gibt eine normale Informationsmeldung aus.
        /// </summary>
        /// <param name="message">Anzuzeigende Information.</param>
        public void PrintInfo(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        /// <summary>
        /// Gibt eine Eingabeaufforderung aus.
        /// </summary>
        /// <param name="message">Aufforderung an den Benutzer.</param>
        public void PrintPrompt(string message)
        {
            Console.Write(message);
        }

        /// <summary>
        /// Wartet darauf, dass der Benutzer eine Taste drückt.
        /// </summary>
        public void WaitForKey()
        {
            Console.WriteLine();
            Console.WriteLine("Drücken Sie eine beliebige Taste, um fortzufahren...");
            Console.ReadKey();
        }

        /// <summary>
        /// Gibt den Fortschritt bei der Verarbeitung eines Bildes aus.
        /// </summary>
        /// <param name="current">Aktuelle Bildnummer.</param>
        /// <param name="total">Gesamtanzahl der Bilder.</param>
        /// <param name="imageName">Name des aktuell verarbeiteten Bildes.</param>
        public void PrintProgress(int current, int total, string imageName)
        {
            Console.WriteLine(
                $"[{current}/{total}] Verarbeite: {imageName}"
            );
        }

        /// <summary>
        /// Gibt die Ergebnisse eines einzelnen OCR-Vergleichs aus.
        /// </summary>
        public void PrintComparisonResult(
            string modelName,
            string imageName,
            double processingTimeMs,
            double cerInPercent)
        {
            Console.WriteLine();
            Console.WriteLine($"Modell: {modelName}");
            Console.WriteLine($"Bild: {imageName}");
            Console.WriteLine($"Verarbeitungszeit: {processingTimeMs:F2} ms");
            Console.WriteLine($"CER: {cerInPercent:F2} %");
        }
    }
}

