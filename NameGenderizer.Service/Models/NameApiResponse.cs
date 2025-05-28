using System.Text.Json.Serialization;

namespace NameGenderizer.Service.Models;

public record NameApiResponse(
    [property: JsonPropertyName("gender")] string Gender = "",
    [property: JsonPropertyName("maleProportion")] double MaleProportion = 0,
    [property: JsonPropertyName("confidence")] double Confidence = 0);