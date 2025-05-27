using CsvHelper;
using CsvHelper.Configuration;
using NameGenderizer.Service.Models;
using NameGenderizer.Service.Extensions;
using System.Globalization;

namespace NameGenderizer.Service.Services;

public class CsvService : ICsvService
{
    private readonly ILogger<CsvService> _logger;

    public CsvService(ILogger<CsvService> logger)
    {
        _logger = logger;
    }

    public async Task<List<Person>> ReadCsvAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Reading CSV file: {FilePath}", filePath);
            
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });
            
            var records = await csv.GetRecordsAsync<Person>(cancellationToken).ToListAsync(cancellationToken);
            
            _logger.LogInformation("Read {Count} records from CSV file", records.Count);
            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading CSV file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task WriteCsvAsync(string filePath, List<Person> people, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Writing {Count} records to CSV file: {FilePath}", people.Count, filePath);
            
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            await csv.WriteRecordsAsync(people, cancellationToken);
            
            _logger.LogInformation("Successfully wrote records to CSV file");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing CSV file: {FilePath}", filePath);
            throw;
        }
    }
}