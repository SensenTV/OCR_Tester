namespace OCR_Tester.Domain.Models
{
    // Repräsentiert den erwarteten Text eines Testbildes.
    // Die Daten stammen aus der vorbereiteten Ground-Truth-JSON-Datei.
    public class GroundTruth
    {
        // Name der zugehörigen Bilddatei.
        public string ImageName { get; set; } = string.Empty;

        // Erwarteter Text für das Bild.
        public string ExpectedText { get; set; } = string.Empty;
    }
}
