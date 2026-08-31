namespace OCR_Tester.Domain.Models
{
    // Repräsentiert einen vollständigen OCR-Testfall.
    // Verknüpft ein Eingabebild mit dem dazugehörigen Ground-Truth-Text.
    public class ImageTestCase
    {
        // Name der Bilddatei.
        public string ImageName { get; set; } = string.Empty;

        // Vollständiger Pfad zur Bilddatei.
        public string ImagePath { get; set; } = string.Empty;
    }
}
