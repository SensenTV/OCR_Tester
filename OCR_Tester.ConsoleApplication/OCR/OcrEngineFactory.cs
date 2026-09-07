using OCR_Tester.Configuration;
using OCR_Tester.Domain.Interfaces;
using OCR_Tester.OCR.GLM;
using OCR_Tester.OCR.Tesseract;

public class OcrEngineFactory
{
    public IOcrEngine Create(OcrEngineSettings settings)
    {
        return settings.Type.ToLowerInvariant() switch
        {
            "glm" => new GlmOcrEngine(
                settings.Endpoint,
                settings.ApiKey,
                settings.Model,
                settings.Prompt
            ),

            "tesseract" => new TesseractOcrEngine(
                settings.TessDataPath,
                settings.Language,
                settings.EngineMode,
                settings.PageSegmentationMode
            ),

            _ => throw new InvalidOperationException(
                $"Unbekannter OCR-Engine-Typ: {settings.Type}")
        };
    }
}