using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NameGenderizer.Service.Models;
using NameGenderizer.Service.Services;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NameGenderizer.Tests;

public class NameGenderizerApiTests
{
    [Fact]
    public async Task GetGenderAsync_WithMaleName_ReturnsMaleGender()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var expectedResponse = new NameGenderizerResponse
        {
            Gender = "MALE",
            MaleProportion = 1.0,
            Confidence = 0.95
        };

        mockHttp
            .When("https://api.nameapi.org/rest/v5.3/genderizer/*")
            .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

        var client = mockHttp.ToHttpClient();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<NameGenderizerApiClient>>();

        var apiClient = new NameGenderizerApiClient(client, configurationMock.Object, loggerMock.Object);

        // Act
        var result = await apiClient.GetGenderAsync("John", "Smith");

        // Assert
        Assert.Equal("MALE", result.Gender);
    }

    [Fact]
    public async Task GetGenderAsync_WithFemaleName_ReturnsFemaleGender()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var expectedResponse = new NameGenderizerResponse
        {
            Gender = "FEMALE",
            MaleProportion = 0.0,
            Confidence = 0.92
        };

        mockHttp
            .When("https://api.nameapi.org/rest/v5.3/genderizer/*")
            .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

        var client = mockHttp.ToHttpClient();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<NameGenderizerApiClient>>();

        var apiClient = new NameGenderizerApiClient(client, configurationMock.Object, loggerMock.Object);

        // Act
        var result = await apiClient.GetGenderAsync("Maria", "Garcia");

        // Assert
        Assert.Equal("FEMALE", result.Gender);
    }
}