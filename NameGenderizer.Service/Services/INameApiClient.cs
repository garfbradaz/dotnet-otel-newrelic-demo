using NameGenderizer.Service.Models;

namespace NameGenderizer.Service.Services;

public interface INameApiClient
{
    Task<NameApiResponse> GetGenderAsync(string firstName, string lastName, CancellationToken cancellationToken = default);
}