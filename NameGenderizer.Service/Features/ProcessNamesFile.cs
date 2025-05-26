using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using NameGenderizer.Service.Models;

namespace NameGenderizer.Service.Features;

public static class ProcessNamesFile
{
    public record Command(string FilePath) : IRequest<Response>;

    public record Response(string OutputFilePath, int ProcessedCount);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<Handler> _logger;

        public Handler(IMediator mediator, ILogger<Handler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing names file: {FilePath}", request.FilePath);

            // Read the input CSV file
            var records = new List<NameGenderRecord>();
            
            try
            {
                using var reader = new StreamReader(request.FilePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
                
                // Read the header record
                csv.Read();
                csv.ReadHeader();
                
                // Process each record
                while (csv.Read())
                {
                    var firstName = csv.GetField("firstname")?.Trim() ?? string.Empty;
                    var lastName = csv.GetField("lastname")?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                    {
                        _logger.LogWarning("Skipping record with missing name data");
                        continue;
                    }

                    _logger.LogInformation("Processing name: {FirstName} {LastName}", firstName, lastName);

                    // Use the GenderizeName feature to get the gender
                    var genderResponse = await _mediator.Send(new GenderizeName.Command(firstName, lastName), cancellationToken);

                    records.Add(new NameGenderRecord
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Gender = genderResponse.Gender
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading or processing names from file {FilePath}", request.FilePath);
                throw;
            }

            // Write the results to a new CSV file
            var directory = Path.GetDirectoryName(request.FilePath) ?? string.Empty;
            var outputPath = Path.Combine(directory, "list_of_names_with_gender.csv");

            try
            {
                using var writer = new StreamWriter(outputPath);
                using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
                
                csv.WriteRecords(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing gender data to output file {OutputPath}", outputPath);
                throw;
            }

            _logger.LogInformation("Completed processing names file. Output saved to {OutputPath}", outputPath);
            return new Response(outputPath, records.Count);
        }
    }
}