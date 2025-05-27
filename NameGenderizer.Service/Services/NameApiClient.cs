using System.Text;
using System.Text.Json;
using NameGenderizer.Service.Models;

namespace NameGenderizer.Service.Services;

public class NameApiClient : INameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NameApiClient> _logger;
    private readonly string _apiKey;
    private readonly string _apiBaseUrl = "https://api.nameapi.org/rest/v5.3/genderizer/persongenderizer";
    
    public NameApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<NameApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration.GetValue<string>("NameApi:ApiKey") ?? "e8b94be37ded6df6e8eb606f304079a8-user1";
    }

    public async Task<NameApiResponse> GetGenderAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new NameApiRequest
            {
                InputPerson = new InputPerson
                {
                    PersonName = new PersonName
                    {
                        NameFields = new List<NameField>
                        {
                            new() { String = firstName, FieldType = "GIVENNAME" },
                            new() { String = lastName, FieldType = "SURNAME" }
                        }
                    }
                }
            };

            var requestJson = JsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var url = $"{_apiBaseUrl}?apiKey={_apiKey}";
            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var nameApiResponse = JsonSerializer.Deserialize<NameApiResponse>(responseContent);

            if (nameApiResponse == null)
            {
                _logger.LogError("Failed to deserialize API response");
                throw new InvalidOperationException("Failed to deserialize API response");
            }

            return nameApiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Name API for {FirstName} {LastName}", firstName, lastName);
            throw;
        }
    }
}