using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PromoPrizeApi.Interfaces;

namespace PromoPrizeApi.Entities;

public class Promotion : IEntity
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("prizes")] public List<Prize> Prizes { get; set; } = new List<Prize>();

    [JsonPropertyName("participants")] public List<Participant> Participants { get; set; } =
        new List<Participant>();

    [JsonPropertyName("id")] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
}