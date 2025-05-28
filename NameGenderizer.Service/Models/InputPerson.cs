using System.Text.Json.Serialization;

namespace NameGenderizer.Service.Models;

public record InputPerson(
    [property: JsonPropertyName("type")] string Type = "NaturalInputPerson",
    [property: JsonPropertyName("personName")] PersonName? PersonName = null,
    [property: JsonPropertyName("gender")] string Gender = "UNKNOWN")
{
    public InputPerson() : this("NaturalInputPerson", new PersonName(), "UNKNOWN") { }
}