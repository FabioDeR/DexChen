using System.Text.Json.Serialization;

namespace DexChen.Domain.Dtos;

public class RoboFlowResponse
{
    [JsonPropertyName("outputs")]
    public List<OutputBlock> Outputs { get; set; } = new();
}

public class OutputBlock
{
    [JsonPropertyName("predictions")]
    public List<RawPrediction> Output { get; set; } = new(); // Ici, on stocke directement les prédictions
}

public class RawPrediction
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("english_name")] public string EnglishName { get; set; } = "";
    [JsonPropertyName("card_number")] public string CardNumber { get; set; } = "";
    [JsonPropertyName("set_code")] public string SetCode { get; set; } = "";
    [JsonPropertyName("original_set_code")] public string OriginalSetCode { get; set; } = "";
    [JsonPropertyName("set_name_english")] public string EnglishSetName { get; set; } = "";
    [JsonPropertyName("language")] public string Language { get; set; } = "";
    [JsonPropertyName("rarity")] public string Rarity { get; set; } = "";
    [JsonPropertyName("english_rarity")] public string EnglishRarity { get; set; } = "";
    [JsonPropertyName("illustrator")] public string Illustrator { get; set; } = "";
    [JsonPropertyName("english_illustrator")] public string EnglishIllustrator { get; set; } = "";
    [JsonPropertyName("confidence_score")] public double ConfidenceScore { get; set; }
    [JsonPropertyName("fields_confidence")]
    public Dictionary<string, double> FieldsConfidence { get; set; } = new();
    [JsonPropertyName("suggested_search_terms")]
    public List<string> SuggestedSearchTerms { get; set; } = new();
    [JsonPropertyName("needs_review")] public bool NeedsReview { get; set; }
    [JsonPropertyName("error_status")] public bool ErrorStatus { get; set; }
}

public class ScanResult
{
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string SetCode { get; set; } = string.Empty;
    public string OriginalSetCode { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public string EnglishRarity { get; set; } = string.Empty;
    public string Illustrator { get; set; } = string.Empty;
    public string EnglishIllustrator { get; set; } = string.Empty;
    public string EnglishSetName { get; set; } = "";

    public double ConfidenceScore { get; set; }
    public Dictionary<string, double> FieldsConfidence { get; set; } = new();
    public List<string> SuggestedSearchTerms { get; set; } = new();
    public bool NeedsReview { get; set; }
    public bool ErrorStatus { get; set; }

    // Ajout pour usage dans le front
    public string ImageUrl { get; set; } = string.Empty;
    public string PriceEstimate { get; set; } = string.Empty;

    // Utilitaire : vérifie si cette prédiction est exploitable
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(CardNumber) &&
        !NeedsReview &&
        !ErrorStatus;
}


