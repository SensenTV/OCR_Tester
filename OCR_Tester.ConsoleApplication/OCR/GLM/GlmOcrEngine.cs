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

        public GlmOcrEngine(string endpoint, string apiKey, string model, string prompt)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException(
                    "Der API-Endpunkt darf nicht leer sein.",
                    nameof(endpoint)
                );
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new ArgumentException("Der Modellname darf nicht leer sein.", nameof(model));
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Der Prompt darf nicht leer sein.", nameof(prompt));
            }

            _prompt = prompt;

            var clientOptions = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };

            _chatClient = new ChatClient(
                model: model,
                credential: new System.ClientModel.ApiKeyCredential(apiKey),
                options: clientOptions
            );
        }

        public async Task<OcrResult> ProcessImageAsync(ImageTestCase testCase)
        {
            var stopwatch = Stopwatch.StartNew();

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

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

                stopwatch.Stop();

                string recognizedText =
                    completion.Content.Count > 0 ? completion.Content[0].Text : string.Empty;

                int inputTokens = completion.Usage?.InputTokenCount ?? 0;
                int outputTokens = completion.Usage?.OutputTokenCount ?? 0;

                var result = new OcrResult
                {
                    ModelName = "GLM-OCR",
                    RecognizedText = recognizedText,
                    ProcessingTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                    InputTokens = inputTokens,
                    OutputTokens = outputTokens,

                    // Wird später anhand der GLM-OCR-Preise berechnet.
                    CostInCents = 0,
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
