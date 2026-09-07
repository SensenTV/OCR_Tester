namespace OCR_Tester.Domain.Models
{
    // Repräsentiert ein eingelesenes Bild.
    public class Image
    {
        // Name der Bilddatei.
        public string ImageName { get; set; } = string.Empty;

        // Vollständiger Pfad zur Bilddatei.
        public string ImagePath { get; set; } = string.Empty;
    }
}
