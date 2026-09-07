namespace OCR_Tester.Domain.Models
{
    // Repräsentiert das Ergebnis einer OCR-Verarbeitung für ein einzelnes Bild.
    // Enthält den erkannten Text sowie Messwerte wie Verarbeitungszeit,
    // Tokenverbrauch und gegebenenfalls die berechneten Kosten.
    public class OcrResult
    {
        public SingleBenchmark SingleBenchmark { get; set; } = new();

        public int CharacterErrorRate => SingleBenchmark.CharacterErrorRate;
        public float CharacterErrorRateInPercent => SingleBenchmark.CharacterErrorRateInPercent;

        // Name des verwendeten OCR-Modells, z. B. "Tesseract" oder "GLM-OCR".
        public string ModelName { get; set; } = string.Empty;

        // Der vom OCR-Modell erkannte Text.
        public string RecognizedText { get; set; } = string.Empty;

        // Verarbeitungszeit für das Bild in Millisekunden.
        public int ProcessingTimeMs { get; set; }

        // Anzahl der verwendeten Eingabetokens.
        // Bei Tesseract nicht verfügbar und daher 0.
        public int InputTokens { get; set; }

        // Anzahl der erzeugten Ausgabetokens.
        // Bei Tesseract nicht verfügbar und daher 0.
        public int OutputTokens { get; set; }

        // Gesamte Anzahl der verwendeten Tokens.
        public int TotalTokens => InputTokens + OutputTokens;
    }
}
