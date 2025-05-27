using NameGenderizer.Service.Services;

namespace NameGenderizer.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly INameApiClient _nameApiClient;
    private readonly ICsvService _csvService;
    private FileSystemWatcher? _fileSystemWatcher;
    private string _watchDirectory = string.Empty;
    private const string FileToWatch = "list_of_names.csv";
    private const string OutputFileName = "list_of_names_with_gender.csv";

    public Worker(ILogger<Worker> logger, IConfiguration configuration, INameApiClient nameApiClient, ICsvService csvService)
    {
        _logger = logger;
        _configuration = configuration;
        _nameApiClient = nameApiClient;
        _csvService = csvService;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _watchDirectory = _configuration.GetValue<string>("WatchSettings:Directory") ?? Path.Combine(Directory.GetCurrentDirectory(), "watchdir");
        
        _logger.LogInformation("Starting service. Watching directory: {Directory}", _watchDirectory);
        
        // Ensure the directory exists
        if (!Directory.Exists(_watchDirectory))
        {
            Directory.CreateDirectory(_watchDirectory);
            _logger.LogInformation("Created watch directory: {Directory}", _watchDirectory);
        }

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

        // Set up file watcher
        _fileSystemWatcher = new FileSystemWatcher
        {
            Path = _watchDirectory,
            Filter = FileToWatch,
            NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        _fileSystemWatcher.Created += OnFileCreated;
        _fileSystemWatcher.Changed += OnFileChanged;
        _fileSystemWatcher.EnableRaisingEvents = true;

        _logger.LogInformation("File watcher set up for {FileName} in {Directory}", FileToWatch, _watchDirectory);

        // Keep the service alive
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping service");
        
        _fileSystemWatcher?.Dispose();
        
        return base.StopAsync(cancellationToken);
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        ProcessFileAsync(e.FullPath, CancellationToken.None).GetAwaiter().GetResult();
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        ProcessFileAsync(e.FullPath, CancellationToken.None).GetAwaiter().GetResult();
    }

    private async Task ProcessFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing file: {FilePath}", filePath);
            
            // 1. Parse the CSV
            var people = await _csvService.ReadCsvAsync(filePath, cancellationToken);
            
            // 2. Call the Name Genderizer API for each person
            foreach (var person in people)
            {
                try
                {
                    var response = await _nameApiClient.GetGenderAsync(
                        person.FirstName.Trim(), 
                        person.LastName.Trim(), 
                        cancellationToken);
                    
                    person.Gender = response.Gender.ToLowerInvariant() switch
                    {
                        "male" => "male",
                        "female" => "female",
                        _ => "unknown"
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting gender for {FirstName} {LastName}", 
                        person.FirstName, person.LastName);
                    person.Gender = "unknown";
                }
            }
            
            // 3. Write the results to a new CSV with genders
            var outputPath = Path.Combine(Path.GetDirectoryName(filePath)!, OutputFileName);
            await _csvService.WriteCsvAsync(outputPath, people, cancellationToken);
            
            _logger.LogInformation("File {FilePath} processed successfully. Output saved to {OutputPath}", 
                filePath, outputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {FilePath}", filePath);
        }
    }
}
