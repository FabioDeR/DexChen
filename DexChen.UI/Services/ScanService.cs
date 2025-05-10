using DexChen.Domain.Dtos;
using DexChen.UI.Services.Contract;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace DexChen.UI.Services
{
    public class ScanService : IScanService
    {
        private string RoboflowUrl = "https://serverless.roboflow.com/dexchen/workflow-dexchen/1?api_key=tNlOnp4iY3tuLoU3luit";
        private readonly HttpClient _httpClient;
        private const double MinGlobalScore = 0.5;
        private const double MinFieldScore = 0.7;

        public ScanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ScanResult?> AnalyzeImageFromBase64Async(string base64Image)
        {
            try
            {
                var requestData = new
                {
                    api_key = "", // Remplacez par votre clé API
                    inputs = new
                    {
                        image = new { type = "base64", value = base64Image }
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestData),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    "https://serverless.roboflow.com/infer/workflows/dexchen/custom-workflow", // Remplacez par votre URL
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur lors de l'appel à Roboflow : {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                return ParsePredictionJson(result); // Réutilisation de votre méthode existante
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de l'image au workflow : {ex.Message}");
                return null;
            }
        }

        public async Task<ScanResult?> AnalyzeUploadedFileAsync(InputFileChangeEventArgs e)
        {
            try
            {

                var requestBody = new
                {
                    api_key = "",
                    inputs = new
                    {
                        image = new
                        {
                            type = "url",
                        }
                    }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erreur lors de l'appel à Roboflow : {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return ParsePredictionJson(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur analyse Roboflow (upload) : {ex.Message}");
                return null;
            }
        }

        public async Task<ScanResult?> AnalyzeImageFromBytesAsync(byte[] bytes, string fileName, string contentType)
        {
            try
            {
                var client = new HttpClient();

                using var content = new MultipartFormDataContent();
                var streamContent = new ByteArrayContent(bytes);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(streamContent, "image", fileName);

                var response = await client.PostAsync(RoboflowUrl, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return ParsePredictionJson(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur analyse Roboflow (bytes) : {ex.Message}");
                return null;
            }
        }

        public ScanResult? ParsePredictionJson(string json)
        {
            try
            {
                // Désérialise la couche RoboFlow
                var rf = JsonSerializer.Deserialize<RoboFlowResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Récupérer toutes les RawPrediction dans les sorties
                var allPreds = rf?.Outputs
                .SelectMany(o => o.Output)
                .Where(p =>
                    !p.ErrorStatus &&
                    p.ConfidenceScore >= MinGlobalScore &&
                    p.FieldsConfidence.TryGetValue("name", out var nameScore) && nameScore >= MinFieldScore &&
                    p.FieldsConfidence.TryGetValue("card_number", out var cardScore) && cardScore >= MinFieldScore
                )
                .ToList();

                if (allPreds is null || !allPreds.Any())
                    return null;

                // Tri par pertinence : pas besoin de ToList ici sauf si besoin plus tard
                var best = allPreds
                    .OrderBy(p => p.NeedsReview)
                    .ThenByDescending(p => p.ConfidenceScore)
                    .First();

                var fallbackTerms = allPreds
                    .Where(p => p != best)
                    .SelectMany(p => p.SuggestedSearchTerms)
                    .Distinct()
                    .ToList();


                return new ScanResult
                {
                    Name = best.Name,
                    EnglishName = best.EnglishName,
                    CardNumber = best.CardNumber,
                    SetCode = best.SetCode,
                    OriginalSetCode = best.OriginalSetCode, // corrigé ici
                    Language = best.Language,
                    Rarity = best.Rarity,
                    EnglishRarity = best.EnglishRarity,
                    Illustrator = best.Illustrator,
                    EnglishIllustrator = best.EnglishIllustrator,
                    ConfidenceScore = best.ConfidenceScore,
                    FieldsConfidence = best.FieldsConfidence,
                    SuggestedSearchTerms = fallbackTerms,
                    EnglishSetName = best.EnglishSetName,
                    NeedsReview = best.NeedsReview,
                    ErrorStatus = best.ErrorStatus,
                    ImageUrl = string.Empty,
                    PriceEstimate = "–"
                };
            }
            catch (JsonException jex)
            {
                Console.WriteLine("Erreur JSON dans ParsePredictionJson : " + jex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur inattendue dans ParsePredictionJson : " + ex.Message);
            }

            return null;
        }

        

    }
}
