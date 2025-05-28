using System.Text.Json.Serialization;

namespace NameGenderizer.Service.Models;

public record NameApiRequest(
    [property: JsonPropertyName("context")] Context Context,
    [property: JsonPropertyName("inputPerson")] InputPerson InputPerson)
{
    public NameApiRequest() : this(new Context(), new InputPerson()) { }
}