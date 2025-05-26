using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NameGenderizer.Service.Services;

namespace NameGenderizer.Tests;

[Collection("LiveApiTests")]
public class LiveApiTests
{
    [Fact]
    public async Task LiveApi_CanDetermineGender_ForMaleName()
    {
        // Arrange
        var httpClient = new HttpClient();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<NameGenderizerApiClient>>();

        var apiClient = new NameGenderizerApiClient(httpClient, configurationMock.Object, loggerMock.Object);

        // Act
        var result = await apiClient.GetGenderAsync("John", "Smith");

        // Assert
        Assert.Equal("MALE", result.Gender);
    }
}