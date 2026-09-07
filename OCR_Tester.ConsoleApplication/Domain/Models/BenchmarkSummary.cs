namespace OCR_Tester.Domain.Models
{
    public class BenchmarkSummary
    {
        public List<BenchmarkModelSummary> Models { get; set; } = [];
    }

    // Enthält die aggregierten Benchmark-Ergebnisse eines OCR-Modells.
    // Dazu gehören unter anderem Gesamtzeit, Durchschnittszeit, CER,
    // Tokenverbrauch und Kosten.
    public class BenchmarkModelSummary
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
        public double OverallCharacterErrorRate { get; set; }

        // Gesamt-CER über alle verarbeiteten Zeichen.
        public double OverallCharacterErrors { get; set; }

        // Gesamtanzahl der verwendeten Eingabetokens.
        public int TotalInputTokens { get; set; }

        // Gesamtanzahl der erzeugten Ausgabetokens.
        public int TotalOutputTokens { get; set; }

        public string GraphicsProcessingUnit { get; set; } = string.Empty;
        public string VRAM { get; set; } = string.Empty;
        public string CPU { get; set; } = string.Empty;
        public string RAM { get; set; } = string.Empty;
    }
}
