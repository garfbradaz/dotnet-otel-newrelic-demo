using System.Text.Json.Serialization;

namespace NameGenderizer.Service.Models;

public record NameField(
    [property: JsonPropertyName("string")] string String = "",
    [property: JsonPropertyName("fieldType")] string FieldType = "")
{
    public NameField() : this("", "") { }
}