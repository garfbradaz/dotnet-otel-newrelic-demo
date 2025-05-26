using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MediatR;
using Moq;
using NameGenderizer.Service;

namespace NameGenderizer.Tests;

public class WorkerTests
{
    [Fact]
    public async Task Worker_StartAsync_CreatesDirectoryIfNotExists()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Worker>>();
        var mediatorMock = new Mock<IMediator>();
        
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
        
        var worker = new Worker(loggerMock.Object, configuration, mediatorMock.Object);
        
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