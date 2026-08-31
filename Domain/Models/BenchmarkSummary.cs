namespace OCR_Tester.Domain.Models
{
    // Enthält die aggregierten Benchmark-Ergebnisse eines OCR-Modells.
    // Dazu gehören unter anderem Gesamtzeit, Durchschnittszeit, CER,
    // Tokenverbrauch und Kosten.
    public class BenchmarkSummary
    {
        // Name des OCR-Modells.
        public string ModelName { get; set; } = string.Empty;

        // Gesamtzahl der verarbeiteten Bilder.
        public int TotalImagesProcessed { get; set; }

        // Gesamte Verarbeitungszeit aller Bilder in Millisekunden.
        public double TotalProcessingTimeMs { get; set; }

        // Durchschnittliche Verarbeitungszeit pro Bild in Millisekunden.
        public double AverageProcessingTimeMs { get; set; }

        // Durchschnittliche CER über alle Bilder in Prozent.
        public double AverageCharacterErrorRateInPercent { get; set; }

        // Gesamt-CER über alle verarbeiteten Zeichen.
        public double OverallCharacterErrorRateInPercent { get; set; }

        // Gesamtanzahl der verwendeten Eingabetokens.
        public int TotalInputTokens { get; set; }

        // Gesamtanzahl der erzeugten Ausgabetokens.
        public int TotalOutputTokens { get; set; }

        // Gesamtkosten für alle verarbeiteten Bilder in Cent.
        public double TotalCostInCents { get; set; }
    }
}
