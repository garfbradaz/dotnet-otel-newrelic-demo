using System.Text;
using System.Text.Json;
using NameGenderizer.Service.Models;

namespace NameGenderizer.Service.Services;

public class NameGenderizerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NameGenderizerApiClient> _logger;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.nameapi.org/rest/v5.3/genderizer/";

    public NameGenderizerApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<NameGenderizerApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = "e8b94be37ded6df6e8eb606f304079a8-user1"; // API key provided in requirements
    }
    
    public async Task<NameGenderizerResponse> GetGenderAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new NameGenderizerRequest
            {
                Context = new Context(),
                InputPerson = new NaturalInputPerson
                {
                    PersonName = new PersonName
                    {
                        NameFields = new List<NameField>
                        {
                            new() { Value = firstName, FieldType = "GIVENNAME" },
                            new() { Value = lastName, FieldType = "SURNAME" }
                        }
                    }
                }
            };

            var url = $"{BaseUrl}?apiKey={_apiKey}";
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            _logger.LogInformation("Calling Name Genderizer API for {FirstName} {LastName}", firstName, lastName);
            
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var genderResponse = JsonSerializer.Deserialize<NameGenderizerResponse>(responseContent);

            if (genderResponse == null)
            {
                _logger.LogWarning("Failed to deserialize gender response for {FirstName} {LastName}", firstName, lastName);
                throw new InvalidOperationException("Failed to deserialize gender response");
            }

            _logger.LogInformation("Gender result for {FirstName} {LastName}: {Gender}", firstName, lastName, genderResponse.Gender);
            return genderResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Name Genderizer API for {FirstName} {LastName}", firstName, lastName);
            throw;
        }
    }
}