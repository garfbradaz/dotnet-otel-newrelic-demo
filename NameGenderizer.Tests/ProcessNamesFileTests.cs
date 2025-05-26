using System.Globalization;
using CsvHelper;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NameGenderizer.Service.Features;
using NameGenderizer.Service.Models;

namespace NameGenderizer.Tests;

public class ProcessNamesFileTests
{
    [Fact]
    public async Task ProcessNamesFile_ReadsInputAndWritesOutput()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var loggerMock = new Mock<ILogger<ProcessNamesFile.Handler>>();
        
        // Create a temporary test file
        var testDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(testDir);
        var inputFilePath = Path.Combine(testDir, "list_of_names.csv");
        var outputFilePath = Path.Combine(testDir, "list_of_names_with_gender.csv");

        // Write test data to the file
        File.WriteAllText(inputFilePath, 
            "firstname,lastname\r\n" +
            "John,Smith\r\n" +
            "Maria,Garcia\r\n");

        // Setup mock responses
        mediatorMock
            .Setup(m => m.Send(It.Is<GenderizeName.Command>(c => c.FirstName == "John" && c.LastName == "Smith"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GenderizeName.Response("male"));
        
        mediatorMock
            .Setup(m => m.Send(It.Is<GenderizeName.Command>(c => c.FirstName == "Maria" && c.LastName == "Garcia"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GenderizeName.Response("female"));

        var handler = new ProcessNamesFile.Handler(mediatorMock.Object, loggerMock.Object);

        try
        {
            // Act
            var result = await handler.Handle(new ProcessNamesFile.Command(inputFilePath), CancellationToken.None);

            // Assert
            Assert.Equal(outputFilePath, result.OutputFilePath);
            Assert.Equal(2, result.ProcessedCount);
            Assert.True(File.Exists(outputFilePath));
            
            // Read the output file and verify content
            var records = new List<NameGenderRecord>();
            using (var reader = new StreamReader(outputFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<NameGenderRecord>().ToList();
            }

            Assert.Equal(2, records.Count);
            Assert.Equal("John", records[0].FirstName);
            Assert.Equal("Smith", records[0].LastName);
            Assert.Equal("male", records[0].Gender);
            
            Assert.Equal("Maria", records[1].FirstName);
            Assert.Equal("Garcia", records[1].LastName);
            Assert.Equal("female", records[1].Gender);

            // Verify that the mediator was called with the correct parameters
            mediatorMock.Verify(m => m.Send(It.Is<GenderizeName.Command>(c => 
                c.FirstName == "John" && c.LastName == "Smith"), It.IsAny<CancellationToken>()), Times.Once);
            
            mediatorMock.Verify(m => m.Send(It.Is<GenderizeName.Command>(c => 
                c.FirstName == "Maria" && c.LastName == "Garcia"), It.IsAny<CancellationToken>()), Times.Once);
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