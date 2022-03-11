using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PromoPrizeApi.Interfaces;

namespace PromoPrizeApi.Entities;

public class Prize : IEntity
{
    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("id")] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
}