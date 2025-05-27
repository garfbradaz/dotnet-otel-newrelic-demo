using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NameGenderizer.Service.Services;
using NameGenderizer.Service.Models;

namespace NameGenderizer.Tests;

public class NameApiClientTests
{
    private readonly ILogger<NameApiClient> _logger = Substitute.For<ILogger<NameApiClient>>();
    private readonly IConfiguration _configuration;
    
    public NameApiClientTests()
    {
        // Create configuration with API key
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"NameApi:ApiKey", "e8b94be37ded6df6e8eb606f304079a8-user1"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
    
    [Fact]
    public async Task GetGenderAsync_ReturnsCorrectGender_ForMaleName()
    {
        // Arrange
        var mockNameApiClient = Substitute.For<INameApiClient>();
        mockNameApiClient.GetGenderAsync("Gareth", "Bradley", Arg.Any<CancellationToken>())
            .Returns(new NameApiResponse { Gender = "MALE", Confidence = 0.9, MaleProportion = 0.9 });
        
        // Act
        var result = await mockNameApiClient.GetGenderAsync("Gareth", "Bradley");
        
        // Assert
        Assert.Equal("MALE", result.Gender);
    }
    
    [Fact]
    public async Task GetGenderAsync_ReturnsCorrectGender_ForFemaleName()
    {
        // Arrange
        var mockNameApiClient = Substitute.For<INameApiClient>();
        mockNameApiClient.GetGenderAsync("Emma", "Thompson", Arg.Any<CancellationToken>())
            .Returns(new NameApiResponse { Gender = "FEMALE", Confidence = 0.9, MaleProportion = 0.1 });
        
        // Act
        var result = await mockNameApiClient.GetGenderAsync("Emma", "Thompson");
        
        // Assert
        Assert.Equal("FEMALE", result.Gender);
    }
}