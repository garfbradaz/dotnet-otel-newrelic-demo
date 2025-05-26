using System.Text.Json.Serialization;

namespace NameGenderizer.Service.Models;

public class NameField
{
    [JsonPropertyName("string")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName("fieldType")]
    public string FieldType { get; set; } = string.Empty;
}

public class PersonName
{
    [JsonPropertyName("nameFields")]
    public List<NameField> NameFields { get; set; } = new();
}

public class NaturalInputPerson
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "NaturalInputPerson";
    
    [JsonPropertyName("personName")]
    public PersonName PersonName { get; set; } = new();
    
    [JsonPropertyName("gender")]
    public string Gender { get; set; } = "UNKNOWN";
}

public class Context
{
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "REALTIME";
    
    [JsonPropertyName("properties")]
    public List<object> Properties { get; set; } = new();
}

public class NameGenderizerRequest
{
    [JsonPropertyName("context")]
    public Context Context { get; set; } = new();
    
    [JsonPropertyName("inputPerson")]
    public NaturalInputPerson InputPerson { get; set; } = new();
}

public class NameGenderizerResponse
{
    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;
    
    [JsonPropertyName("maleProportion")]
    public double MaleProportion { get; set; }
    
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }
}

public class NameGenderRecord
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
}