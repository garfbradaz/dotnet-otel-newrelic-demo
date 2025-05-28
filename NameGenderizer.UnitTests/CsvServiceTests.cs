using Microsoft.Extensions.Logging;
using NameGenderizer.Service.Models;
using NameGenderizer.Service.Services;
using NSubstitute;
using System.IO;
using System.Text.RegularExpressions;

namespace NameGenderizer.UnitTests;

public class CsvServiceTests
{
    private readonly ILogger<CsvService> _logger = Substitute.For<ILogger<CsvService>>();
    
    [Fact]
    public async Task ReadCsvAsync_ReadsPeopleCorrectly()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "firstname,lastname\r\nGareth,Bradley\r\nEmma,Thompson");
        
        var csvService = new CsvService(_logger);
        
        try
        {
            // Act
            var result = await csvService.ReadCsvAsync(tempFile);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Gareth", result[0].FirstName);
            Assert.Equal("Bradley", result[0].LastName);
            Assert.Equal("Emma", result[1].FirstName);
            Assert.Equal("Thompson", result[1].LastName);
        }
        finally
        {
            // Clean up
            File.Delete(tempFile);
        }
    }
    
    [Fact]
    public async Task WriteCsvAsync_WritesPeopleWithGenders()
    {
        // Arrange
        var people = new List<Person>
        {
            new() { FirstName = "Gareth", LastName = "Bradley", Gender = "male" },
            new() { FirstName = "Emma", LastName = "Thompson", Gender = "female" }
        };
        
        var tempFile = Path.GetTempFileName();
        var csvService = new CsvService(_logger);
        
        try
        {
            // Act
            await csvService.WriteCsvAsync(tempFile, people);
            
            // Assert
            var fileContent = await File.ReadAllTextAsync(tempFile);
            
            // Check case-insensitively
            Assert.Matches(new Regex("firstname,lastname,gender", RegexOptions.IgnoreCase), fileContent);
            Assert.Contains("Gareth,Bradley,male", fileContent);
            Assert.Contains("Emma,Thompson,female", fileContent);
        }
        finally
        {
            // Clean up
            File.Delete(tempFile);
        }
    }
}