using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR_Tester.Domain.Interfaces;
using OCR_Tester.Domain.Models;
using Serilog;

namespace OCR_Tester.Evaluation
{
    // Berechnet die Character Error Rate (CER) zwischen einem erwarteten
    // Ground-Truth-Text und einem von einem OCR-Modell erkannten Text.
    // Die Berechnung basiert auf der Levenshtein-Edit-Distance.
    public class CerCalculator : IEvaluationService<SingleBenchmark>
    {
        /// <summary>
        ///     Calculate the difference between 2 strings using the Levenshtein distance algorithm
        /// </summary>
        /// <param name="expectedText">First string</param>
        /// <param name="ocrResult">Second string</param>
        /// <returns></returns>
        public static int CalculateCer(string expectedText, string ocrResult) //O(n*m)
        {
            var expectedTextLength = expectedText.Replace("\r", "").Replace("\n", "").Length;
            var ocrResultLength = ocrResult.Replace("\r", "").Replace("\n", "").Length;

            var matrix = new int[expectedTextLength + 1, ocrResultLength + 1];

            // First calculation, if one entry is empty return full length
            if (expectedTextLength == 0)
                return ocrResultLength;

            if (ocrResultLength == 0)
                return expectedTextLength;

            // Initialization of matrix with row size expectedTextLength and columns size ocrResultLength
            for (var i = 0; i <= expectedTextLength; matrix[i, 0] = i++) { }
            for (var j = 0; j <= ocrResultLength; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= expectedTextLength; i++)
            {
                for (var j = 1; j <= ocrResultLength; j++)
                {
                    var cost = (ocrResult[j - 1] == expectedText[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }
            // return result
            Log.Information("Calculated CER between expected text and OCR result");
            return matrix[expectedTextLength, ocrResultLength];
        }

        public static float CalculateCerInPercent(string expectedText, string ocrResult)
        {
            var editDistance = CalculateCer(expectedText, ocrResult);
            var expectedTextLength = expectedText.Replace("\r", "").Replace("\n", "").Length;

            if (expectedTextLength == 0)
            {
                return 0.0f;
            }

            float result = (float)editDistance / expectedTextLength * 100;

            Log.Information("Calculated CER in percent between expected text and OCR result");
            return MathF.Round(result, 2);
        }

        /// <summary> /// Erstellt ein SingleBenchmark-Objekt mit den CER-Ergebnissen. /// </summary>
        /// <param name="expectedText">Der erwartete Text.</param>
        /// <param name="ocrResult">Das von dem OCR-Modell erkannte Text.</param>
        /// <returns>Ein neues SingleBenchmark-Objekt mit den CER-Ergebnissen.</returns>
        public SingleBenchmark Calculate(string expectedText, string ocrResult)
        {
            var editDistance = CalculateCer(expectedText, ocrResult);
            var cerInPercent = CalculateCerInPercent(expectedText, ocrResult);
            return new SingleBenchmark
            {
                CharacterErrorRate = editDistance,
                CharacterErrorRateInPercent = cerInPercent,
            };
        }
    }
}
