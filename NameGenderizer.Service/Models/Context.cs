using System.Text.Json.Serialization;

namespace NameGenderizer.Service.Models;

public record Context(
    [property: JsonPropertyName("priority")] string Priority = "REALTIME",
    [property: JsonPropertyName("properties")] List<object>? Properties = null)
{
    public Context() : this("REALTIME", new List<object>()) { }
}