using Tesseract;

namespace OCR_Tester.Configuration
{
    public class AppSettings
    {
        public List<OcrEngineSettings> OcrEngines { get; set; } = [];
    }

    public class OcrEngineSettings
    {
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Endpoint { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Prompt { get; set; } = string.Empty;

        public string TessDataPath { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public Tesseract.EngineMode EngineMode { get; set; }

        public Tesseract.PageSegMode PageSegmentationMode { get; set; }

        public string GraphicsProcessingUnit { get; set; } = string.Empty;

        public string VRAM { get; set; } = string.Empty;

        public string CPU { get; set; } = string.Empty;

        public string RAM { get; set; } = string.Empty;
    }
}
