using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OCR_Tester.Domain.Interfaces;
using OCR_Tester.Domain.Models;
using Tesseract;

namespace OCR_Tester.OCR.Tesseract
{
    /// <summary>
    /// Führt die OCR-Erkennung mit Tesseract durch.
    /// </summary>
    public class TesseractOcrEngine : IOcrEngine
    {
        private readonly string _tessDataPath;
        private readonly string _language;
        private readonly EngineMode _engineMode;
        private readonly PageSegMode _pageSegmentationMode;
        public string ModelName => "Tesseract";

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="TesseractOcrEngine"/>-Klasse.
        /// </summary>
        /// <param name="tessDataPath"></param>
        /// <param name="language"></param>
        /// <param name="engineMode"></param>
        /// <param name="pageSegmentationMode"></param>
        public TesseractOcrEngine(
            string tessDataPath,
            string language,
            EngineMode engineMode,
            PageSegMode pageSegmentationMode
        )
        {
            _tessDataPath = tessDataPath;
            _language = language;
            _engineMode = engineMode;
            _pageSegmentationMode = pageSegmentationMode;
        }

        /// <summary>
        /// Verarbeitet ein einzelnes Bild mit Tesseract.
        /// </summary>
        /// <param name="testCase">Der Testfall mit dem zu verarbeitenden Bild.</param>
        /// <returns>Das Ergebnis der OCR-Verarbeitung.</returns>
        public Task<OcrResult> ProcessImageAsync(ImageTestCase testCase)
        {
            if (string.IsNullOrWhiteSpace(testCase.ImagePath))
            {
                throw new ArgumentException(
                    "Der Bildpfad darf nicht leer sein.",
                    nameof(testCase.ImagePath)
                );
            }

            if (!File.Exists(testCase.ImagePath))
            {
                throw new FileNotFoundException(
                    "Das angegebene Bild wurde nicht gefunden.",
                    testCase.ImagePath
                );
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var engine = new TesseractEngine(_tessDataPath, _language, _engineMode);

                engine.DefaultPageSegMode = _pageSegmentationMode;

                using var image = Pix.LoadFromFile(testCase.ImagePath);
                using var page = engine.Process(image);

                string recognizedText = page.GetText();

                stopwatch.Stop();

                var result = new OcrResult
                {
                    ModelName = "Tesseract",
                    RecognizedText = recognizedText,
                    ProcessingTimeMs = (int)stopwatch.Elapsed.TotalMilliseconds,
                };

                return Task.FromResult(result);
            }
            catch
            {
                stopwatch.Stop();
                throw;
            }
        }
    }
}
