using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NameGenderizer.Service;
using NameGenderizer.Service.Models;
using NameGenderizer.Service.Services;
using NSubstitute;
using System.Text.RegularExpressions;

namespace NameGenderizer.IntegrationTests;

public class WorkerIntegrationTests
{
    [Fact]
    public async Task ProcessFile_GeneratesOutputFile_WithGenders()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        
        try
        {
            // Create test CSV file
            var inputFile = Path.Combine(testDir, "list_of_names.csv");
            await File.WriteAllTextAsync(inputFile, "firstname,lastname\r\nGareth,Bradley\r\nEmma,Thompson");
            
            var outputFile = Path.Combine(testDir, "list_of_names_with_gender.csv");
            
            // Create configuration
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"WatchSettings:Directory", testDir},
                {"NameApi:ApiKey", "e8b94be37ded6df6e8eb606f304079a8-user1"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            
            // Create mock service with predefined responses
            var nameApiClient = Substitute.For<INameApiClient>();
            nameApiClient.GetGenderAsync("Gareth", "Bradley", Arg.Any<CancellationToken>())
                .Returns(new NameApiResponse { Gender = "MALE" });
            nameApiClient.GetGenderAsync("Emma", "Thompson", Arg.Any<CancellationToken>())
                .Returns(new NameApiResponse { Gender = "FEMALE" });
            
            var logger = Substitute.For<ILogger<Worker>>();
            var csvServiceLogger = Substitute.For<ILogger<CsvService>>();
            
            // Use real CSV service
            var csvService = new CsvService(csvServiceLogger);
            
            var worker = new Worker(logger, configuration, nameApiClient, csvService);
            
            // Act
            await worker.StartAsync(CancellationToken.None);
            
            // Directly invoke the private method we want to test using reflection
            var methodInfo = typeof(Worker).GetMethod("ProcessFileAsync", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            await (Task)methodInfo!.Invoke(worker, new object[] { inputFile, CancellationToken.None })!;
            
            // Assert
            Assert.True(File.Exists(outputFile), "Output file should exist");
            
            var fileContent = await File.ReadAllTextAsync(outputFile);
            
            // Check case-insensitively 
            Assert.Matches(new Regex("firstname,lastname,gender", RegexOptions.IgnoreCase), fileContent);
            Assert.Contains("Gareth,Bradley,male", fileContent);
            Assert.Contains("Emma,Thompson,female", fileContent);
            
            await worker.StopAsync(CancellationToken.None);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
        }
    }
}