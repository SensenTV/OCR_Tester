using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Tester.Domain.Models
{
    // Repräsentiert das Ergebnis des Vergleichs zwischen einem OCR-Ergebnis
    // und der zugehörigen Ground Truth. Enthält unter anderem Edit Distance
    // und Character Error Rate (CER).
    public class ComparisonResult
    {
        // Name der verglichenen Bilddatei.
        public string ImageName { get; set; } = string.Empty;

        // Name des verwendeten OCR-Modells.
        public string ModelName { get; set; } = string.Empty;

        // Vom OCR-Modell erkannter Text.
        public string RecognizedText { get; set; } = string.Empty;

        // Erwarteter Text aus der Ground Truth.
        public string ExpectedText { get; set; } = string.Empty;

        // Levenshtein-Distanz zwischen Ground Truth und OCR-Ergebnis.
        public int EditDistance { get; set; }

        // Character Error Rate als absolute Anzahl der Zeichenfehler.
        public int CharacterErrors { get; set; }

        // Character Error Rate in Prozent.
        public double CharacterErrorRateInPercent { get; set; }
    }
}
