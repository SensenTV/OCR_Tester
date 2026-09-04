using System.Text.Json;
using OCR_Tester.Domain.Models;
using Serilog;

namespace OCR_Tester.Input
{
    /// <summary>
    /// Lädt die für den OCR-Vergleich vorgesehenen BMP-Bilder und die zugehörigen Ground-Truth-JSON-Daten.
    /// </summary>
    public class DataLoader
    {
        private string ReadFolderPath()
        {
            while (true)
            {
                Console.WriteLine("Bitte den Pfad des Ordners eingeben:");
                string? folderPath = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(folderPath))
                {
                    Log.Warning("Der Ordnerpfad darf nicht leer sein.");
                    Console.WriteLine("Der Ordnerpfad darf nicht leer sein.");
                    continue;
                }

                if (!Directory.Exists(folderPath))
                {
                    Log.Warning(
                        "Der angegebene Ordner wurde nicht gefunden: {FolderPath}",
                        folderPath
                    );
                    Console.WriteLine("Der angegebene Ordner wurde nicht gefunden.");
                    continue;
                }

                return folderPath;
            }
        }

        private List<Image> LoadImagesFromFolder(string folderPath)
        {
            var images = new List<Image>();
            foreach (
                var file in Directory.GetFiles(folderPath, "*.bmp", SearchOption.AllDirectories)
            )
            {
                images.Add(new Image { ImageName = Path.GetFileName(file), ImagePath = file });
                bool isEmpty = !images.Any();
                if (isEmpty)
                {
                    Log.Warning(
                        "No BMP images found in the specified folder: {FolderPath}",
                        folderPath
                    );
                    Console.WriteLine("Warnung: Keine BMP-Bilder im angegebenen Ordner gefunden.");
                }
            }
            return images;
        }

        private List<GroundTruth> LoadGroundTruthFromJson(string folderPath)
        {
            var groundTruths = new List<GroundTruth>();

            foreach (
                var file in Directory.GetFiles(folderPath, "*.json", SearchOption.AllDirectories)
            )
            {
                string json = File.ReadAllText(file);

                Dictionary<string, GroundTruth>? data = JsonSerializer.Deserialize<
                    Dictionary<string, GroundTruth>
                >(json);

                if (data != null)
                {
                    groundTruths.AddRange(data.Values);
                }
                else
                {
                    Log.Warning("No valid JSON data found in the file: {FilePath}", file);
                    Console.WriteLine(
                        $"Warnung: Keine gültigen JSON-Daten in der Datei '{file}' gefunden."
                    );
                }
            }

            return groundTruths;
        }

        public List<ImageTestCase> LoadImageTestCases()
        {
            string folderPath = ReadFolderPath();

            var images = LoadImagesFromFolder(folderPath);
            var groundTruths = LoadGroundTruthFromJson(folderPath);

            var testCases = new List<ImageTestCase>();

            foreach (var image in images)
            {
                var matchingGroundTruth = groundTruths.FirstOrDefault(gt =>
                    gt.ImageName.Equals(image.ImageName, StringComparison.OrdinalIgnoreCase)
                );

                if (matchingGroundTruth != null)
                {
                    testCases.Add(
                        new ImageTestCase
                        {
                            ImageName = image.ImageName,
                            ImagePath = image.ImagePath,
                            ExpectedText = matchingGroundTruth.ExpectedText,
                        }
                    );
                }
                else
                {
                    Log.Warning(
                        "Kein Ground-Truth-Text für das Bild '{ImageName}' gefunden.",
                        image.ImageName
                    );
                    Console.WriteLine(
                        $"Warnung: Kein Ground-Truth-Text für das Bild '{image.ImageName}' gefunden."
                    );
                }
            }

            return testCases;
        }
    }
}
