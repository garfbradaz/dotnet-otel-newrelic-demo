using System.IO;

namespace NameGenderizer.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private FileSystemWatcher? _fileSystemWatcher;
    private string _watchDirectory = string.Empty;
    private const string FileToWatch = "list_of_names.csv";

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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
        ProcessFile(e.FullPath);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        ProcessFile(e.FullPath);
    }

    private void ProcessFile(string filePath)
    {
        try
        {
            _logger.LogInformation("Processing file: {FilePath}", filePath);
            // In a real implementation, this would:
            // 1. Parse the CSV
            // 2. Call the Name Genderizer API
            // 3. Write the results to a new CSV with genders
            
            // For now, just log the detection
            _logger.LogInformation("File {FilePath} detected and would be processed", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {FilePath}", filePath);
        }
    }
}
