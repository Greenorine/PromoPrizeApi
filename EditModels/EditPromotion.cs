using System.Text.Json.Serialization;

namespace PromoPrizeApi.EditModels;

public class EditPromotion
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}