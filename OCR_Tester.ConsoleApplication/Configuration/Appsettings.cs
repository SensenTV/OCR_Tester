using Tesseract;

namespace OCR_Tester.Configuration
{
    public class AppSettings
    {
        public TesseractSettings Tesseract { get; set; } = new();
        public GlmSettings GlmOcr { get; set; } = new();
    }

    public class TesseractSettings
    {
        public string TessDataPath { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public EngineMode EngineMode { get; set; } = EngineMode.LstmOnly;
        public PageSegMode PageSegmentationMode { get; set; } = PageSegMode.SingleBlock;
    }

    public class GlmSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
    }
}
