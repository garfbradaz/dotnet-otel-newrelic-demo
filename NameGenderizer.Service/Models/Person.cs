using CsvHelper.Configuration.Attributes;

namespace NameGenderizer.Service.Models;

public class Person
{
    [Name("firstname")]
    public string FirstName { get; set; } = string.Empty;
    
    [Name("lastname")]
    public string LastName { get; set; } = string.Empty;
    
    [Name("gender")]
    public string? Gender { get; set; }
}