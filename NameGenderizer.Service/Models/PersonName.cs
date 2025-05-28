using System.Text.Json.Serialization;

namespace NameGenderizer.Service.Models;

public record PersonName(
    [property: JsonPropertyName("nameFields")] List<NameField>? NameFields = null)
{
    public PersonName() : this(new List<NameField>()) { }
}