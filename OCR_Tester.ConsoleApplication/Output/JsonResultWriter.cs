using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Tester.Output
{
    // Schreibt die vollständigen Benchmark- und Vergleichsergebnisse
    // in eine strukturierte JSON-Ergebnisdatei.
    public class JsonResultWriter
    {
        public void WriteResultsToJsonFile(string filePath, object results)
        {
            string jsonString = System.Text.Json.JsonSerializer.Serialize(
                results,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true, // Für eine lesbare Formatierung
                }
            );
            System.IO.File.WriteAllText(filePath, jsonString);
        }
    }
}
