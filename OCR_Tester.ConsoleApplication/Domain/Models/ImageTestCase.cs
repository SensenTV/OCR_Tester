namespace OCR_Tester.Domain.Models
{
    // Repräsentiert einen vollständigen OCR-Testfall.
    // Verknüpft ein Eingabebild mit dem dazugehörigen Ground-Truth-Text.
    public class ImageTestCase : Image
    {
        public GroundTruth GroundTruth { get; set; } = new();
    }
}
