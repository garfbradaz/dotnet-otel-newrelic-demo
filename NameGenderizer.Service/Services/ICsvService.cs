using NameGenderizer.Service.Models;

namespace NameGenderizer.Service.Services;

public interface ICsvService
{
    Task<List<Person>> ReadCsvAsync(string filePath, CancellationToken cancellationToken = default);
    Task WriteCsvAsync(string filePath, List<Person> people, CancellationToken cancellationToken = default);
}