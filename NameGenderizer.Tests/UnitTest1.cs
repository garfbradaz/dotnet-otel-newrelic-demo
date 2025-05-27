using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NameGenderizer.Service;
using NameGenderizer.Service.Services;
using NSubstitute;

namespace NameGenderizer.Tests;

public class WorkerTests
{
    [Fact]
    public async Task Worker_StartAsync_CreatesDirectoryIfNotExists()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Worker>>();
        
        // Create a configuration with test settings
        var testDir = Path.Combine(Path.GetTempPath(), "test_watchdir");
        
        // Clean up any existing directory before the test
        if (Directory.Exists(testDir))
        {
            Directory.Delete(testDir, true);
        }
        
        var inMemorySettings = new Dictionary<string, string?> {
            {"WatchSettings:Directory", testDir}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        var nameApiClient = Substitute.For<INameApiClient>();
        var csvService = Substitute.For<ICsvService>();
        
        var worker = new Worker(loggerMock.Object, configuration, nameApiClient, csvService);
        
        try
        {
            // Act
            await worker.StartAsync(CancellationToken.None);
            
            // Assert
            Assert.True(Directory.Exists(testDir));
        }
        finally
        {
            // Cleanup
            await worker.StopAsync(CancellationToken.None);
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
        }
    }
}