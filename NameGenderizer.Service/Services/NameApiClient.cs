using System.Text;
using System.Text.Json;
using NameGenderizer.Service.Models;
using NameGenderizer.Service.Extensions;

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
            var request = CreateNameApiRequest(firstName, lastName);
            var content = SerializeRequest(request);
            var response = await SendApiRequestAsync(content, cancellationToken);
            return await DeserializeResponseAsync(response, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogNameApiError(ex, firstName, lastName);
            throw;
        }
    }

    private NameApiRequest CreateNameApiRequest(string firstName, string lastName)
    {
        return new NameApiRequest(
            new Context("REALTIME", new List<object>()),
            new InputPerson(
                "NaturalInputPerson",
                new PersonName(new List<NameField>
                {
                    new NameField(firstName, "GIVENNAME"),
                    new NameField(lastName, "SURNAME")
                }),
                "UNKNOWN"
            )
        );
    }

    private StringContent SerializeRequest(NameApiRequest request)
    {
        var requestJson = JsonSerializer.Serialize(request);
        return new StringContent(requestJson, Encoding.UTF8, "application/json");
    }

    private async Task<HttpResponseMessage> SendApiRequestAsync(StringContent content, CancellationToken cancellationToken)
    {
        var url = $"{_apiBaseUrl}?apiKey={_apiKey}";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response;
    }

    private async Task<NameApiResponse> DeserializeResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var nameApiResponse = JsonSerializer.Deserialize<NameApiResponse>(responseContent);

            if (nameApiResponse == null)
            {
                throw new JsonException("API returned null response after deserialization");
            }

            return nameApiResponse;
        }
        catch (Exception)
        {
            _logger.LogDeserializeError();
            throw; // This preserves the original stack trace
        }
    }
}