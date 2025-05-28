using CsvHelper.Configuration.Attributes;

namespace NameGenderizer.Service.Models;

// Person is a partially mutable record - the core identity properties (FirstName, LastName) are immutable,
// but Gender needs to be mutable as it's set after CSV deserialization
public record Person
{
    [Name("firstname")]
    public string FirstName { get; init; } = string.Empty;
    
    [Name("lastname")]
    public string LastName { get; init; } = string.Empty;
    
    [Name("gender")]
    public string? Gender { get; set; }
    
    public Person() { }
    
    public Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}