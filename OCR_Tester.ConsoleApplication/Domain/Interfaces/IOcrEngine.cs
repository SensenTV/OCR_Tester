using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR_Tester.Domain.Models;

namespace OCR_Tester.Domain.Interfaces
{
    // Definiert die gemeinsame Schnittstelle für OCR-Engines.
    // Ermöglicht die einheitliche Verarbeitung von Tesseract und GLM-OCR,
    // unabhängig von deren konkreter Implementierung.
    public interface IOcrEngine
    {
        Task<OcrResult> ProcessImageAsync(ImageTestCase testCase);
    }
}
