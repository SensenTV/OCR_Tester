using System.Diagnostics;
using System.Text.Json;
using OCR_Tester.Domain.Interfaces;
using OCR_Tester.Domain.Models;
using OpenAI;
using OpenAI.Chat;
using Serilog;

namespace OCR_Tester.OCR.GLM
{
    /// <summary>
    /// Führt die OCR-Erkennung mit GLM-OCR über eine OpenAI-kompatible API aus.
    /// </summary>
    public class GlmOcrEngine : IOcrEngine
    {
        private readonly ChatClient _chatClient;
        private readonly string _prompt;
        public string ModelName => "GLM-OCR";

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="GlmOcrEngine"/>-Klasse.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="apiKey"></param>
        /// <param name="model"></param>
        /// <param name="prompt"></param>
        /// <exception cref="ArgumentException"></exception>
        public GlmOcrEngine(string endpoint, string apiKey, string model, string prompt)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                Log.Error("API endpoint is empty. Exiting the GLM OCR engine initialization.");
                throw new ArgumentException(
                    "Der API-Endpunkt darf nicht leer sein. Füge ihn deiner Environment-Variable hinzu. Siehe README.md für weitere Informationen.",
                    nameof(endpoint)
                );
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                Log.Error("Model name is empty. Exiting the GLM OCR engine initialization.");
                throw new ArgumentException(
                    "Der Modellname darf nicht leer sein. Füge ihn in die appsettings.json-Datei hinzu.",
                    nameof(model)
                );
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                Log.Error("Prompt is empty. Exiting the GLM OCR engine initialization.");
                throw new ArgumentException(
                    "Der Prompt darf nicht leer sein. Füge es in den appsettings.json-Datei hinzu.",
                    nameof(prompt)
                );
            }

            _prompt = prompt;

            var clientOptions = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };

            _chatClient = new ChatClient(
                model: model,
                credential: new System.ClientModel.ApiKeyCredential(apiKey),
                options: clientOptions
            );
        }

        /// <summary>
        /// Verarbeitet ein Bild mit GLM-OCR.
        /// </summary>
        /// <param name="testCase">Der Testfall mit dem zu verarbeitenden Bild.</param>
        /// <returns>Das Ergebnis der OCR-Verarbeitung.</returns>
        public async Task<OcrResult> ProcessImageAsync(ImageTestCase testCase)
        {
            var stopwatch = Stopwatch.StartNew();
            ChatCompletion? completion = null;

            try
            {
                byte[] imageBytes = await File.ReadAllBytesAsync(testCase.ImagePath);

                BinaryData imageData = BinaryData.FromBytes(imageBytes);

                List<ChatMessage> messages =
                [
                    new UserChatMessage([
                        ChatMessageContentPart.CreateTextPart(_prompt),
                        ChatMessageContentPart.CreateImagePart(imageData, "image/bmp"),
                    ]),
                ];

                try
                {
                    Log.Information(
                        "Sending image to GLM-OCR for processing: {ImagePath}",
                        testCase.ImagePath
                    );
                    completion = await _chatClient.CompleteChatAsync(messages);
                }
                catch (Exception ex)
                {
                    Log.Error(
                        ex,
                        "Error occurred while sending image to GLM-OCR: {ImagePath}",
                        testCase.ImagePath
                    );
                    throw new InvalidOperationException(
                        $"Fehler beim Senden des Bildes an GLM-OCR: {testCase.ImagePath}. Überprüfe die appsettings.json, ob alle Einstellungen korrekt sind.",
                        ex
                    );
                }

                stopwatch.Stop();

                string recognizedText =
                    completion.Content.Count > 0 ? completion.Content[0].Text : string.Empty;

                int inputTokens = completion.Usage?.InputTokenCount ?? 0;
                int outputTokens = completion.Usage?.OutputTokenCount ?? 0;

                var result = new OcrResult
                {
                    ModelName = "GLM-OCR",
                    RecognizedText = recognizedText,
                    ProcessingTimeMs = stopwatch.Elapsed.Milliseconds,
                    InputTokens = inputTokens,
                    OutputTokens = outputTokens,
                };

                Log.Information(
                    "GLM-OCR verarbeitet Bild {ImagePath} in {ProcessingTimeMs:F2} ms.",
                    testCase.ImagePath,
                    result.ProcessingTimeMs
                );

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                Log.Error(
                    ex,
                    "Fehler bei der Verarbeitung des Bildes mit GLM-OCR: {ImagePath}",
                    testCase.ImagePath
                );

                throw;
            }
        }
    }
}
