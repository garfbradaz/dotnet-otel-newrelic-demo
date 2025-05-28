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
            _logger.LogReadingCsvFile(filePath);
            
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });
            
            var records = await csv.GetRecordsAsync<Person>(cancellationToken).ToListAsync(cancellationToken);
            
            _logger.LogReadCsvRecordsCount(records.Count);
            return records;
        }
        catch (Exception ex)
        {
            _logger.LogReadCsvError(ex, filePath);
            throw;
        }
    }

    public async Task WriteCsvAsync(string filePath, List<Person> people, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWritingCsvFile(people.Count, filePath);
            
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            await csv.WriteRecordsAsync(people, cancellationToken);
            
            _logger.LogWroteCsvFile();
        }
        catch (Exception ex)
        {
            _logger.LogWriteCsvError(ex, filePath);
            throw;
        }
    }
}