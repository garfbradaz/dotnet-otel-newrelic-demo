using MediatR;
using NameGenderizer.Service.Models;
using NameGenderizer.Service.Services;

namespace NameGenderizer.Service.Features;

public static class GenderizeName
{
    public record Command(string FirstName, string LastName) : IRequest<Response>;

    public record Response(string Gender);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly NameGenderizerApiClient _apiClient;
        private readonly ILogger<Handler> _logger;

        public Handler(NameGenderizerApiClient apiClient, ILogger<Handler> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Genderizing name: {FirstName} {LastName}", request.FirstName, request.LastName);

            var response = await _apiClient.GetGenderAsync(request.FirstName, request.LastName, cancellationToken);
            
            // Normalize the gender string to lowercase for consistency
            var gender = response.Gender.ToLower() switch
            {
                "male" => "male",
                "female" => "female",
                _ => "unknown"
            };

            return new Response(gender);
        }
    }
}