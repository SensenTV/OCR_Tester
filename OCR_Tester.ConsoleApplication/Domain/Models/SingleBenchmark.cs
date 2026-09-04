namespace OCR_Tester.Domain.Models
{
    // Enthält die aggregierten Benchmark-Ergebnisse eines OCR-Modells.
    // Dazu gehören unter anderem Gesamtzeit, Durchschnittszeit, CER,
    // Tokenverbrauch und Kosten.
    public class SingleBenchmark : OcrResult
    {
        public int CharacterErrorRate { get; set; }

        public float CharacterErrorRateInPercent { get; set; }
    }
}
