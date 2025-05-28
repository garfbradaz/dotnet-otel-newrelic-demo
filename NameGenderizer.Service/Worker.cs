using NameGenderizer.Service.Services;
using NameGenderizer.Service.Extensions;
using NameGenderizer.Service.Models;

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
        
        _logger.LogStartingService(_watchDirectory);
        
        EnsureWatchDirectoryExists();

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWorkerStarted(DateTimeOffset.Now);

        SetupFileWatcher();

        // Keep the service alive
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogStoppingService();
        
        _fileSystemWatcher?.Dispose();
        
        return base.StopAsync(cancellationToken);
    }

    private void EnsureWatchDirectoryExists()
    {
        if (!Directory.Exists(_watchDirectory))
        {
            Directory.CreateDirectory(_watchDirectory);
            _logger.LogCreatedWatchDirectory(_watchDirectory);
        }
    }

    private void SetupFileWatcher()
    {
        _fileSystemWatcher = new FileSystemWatcher
        {
            Path = _watchDirectory,
            Filter = FileToWatch,
            NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        _fileSystemWatcher.Created += OnFileCreated;
        _fileSystemWatcher.Changed += OnFileChanged;
        _fileSystemWatcher.EnableRaisingEvents = true;

        _logger.LogFileWatcherSetup(FileToWatch, _watchDirectory);
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
            _logger.LogProcessingFile(filePath);
            
            var people = await ReadCsvFileAsync(filePath, cancellationToken);
            await EnrichPeopleWithGenderAsync(people, cancellationToken);
            await WriteCsvResultsAsync(filePath, people, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogProcessingFileError(ex, filePath);
        }
    }

    private async Task<List<Person>> ReadCsvFileAsync(string filePath, CancellationToken cancellationToken)
    {
        return await _csvService.ReadCsvAsync(filePath, cancellationToken);
    }

    private async Task EnrichPeopleWithGenderAsync(List<Person> people, CancellationToken cancellationToken)
    {
        foreach (var person in people)
        {
            try
            {
                var response = await _nameApiClient.GetGenderAsync(
                    person.FirstName.Trim(), 
                    person.LastName.Trim(), 
                    cancellationToken);
                
                person.Gender = MapGenderResponse(response.Gender);
            }
            catch (Exception ex)
            {
                _logger.LogGettingGenderError(ex, person.FirstName, person.LastName);
                person.Gender = "unknown";
            }
        }
    }

    private string MapGenderResponse(string apiGender)
    {
        return apiGender.ToLowerInvariant() switch
        {
            "male" => "male",
            "female" => "female",
            _ => "unknown"
        };
    }

    private async Task WriteCsvResultsAsync(string inputFilePath, List<Person> people, CancellationToken cancellationToken)
    {
        var outputPath = Path.Combine(Path.GetDirectoryName(inputFilePath)!, OutputFileName);
        await _csvService.WriteCsvAsync(outputPath, people, cancellationToken);
        
        _logger.LogFileProcessedSuccessfully(inputFilePath, outputPath);
    }
}
