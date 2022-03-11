using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PromoPrizeApi.Interfaces;

namespace PromoPrizeApi.Entities;

public class Participant : IEntity
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("id")] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
}