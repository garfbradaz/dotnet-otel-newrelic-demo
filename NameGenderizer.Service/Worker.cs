using System.IO;
using MediatR;
using NameGenderizer.Service.Features;

namespace NameGenderizer.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    private FileSystemWatcher? _fileSystemWatcher;
    private string _watchDirectory = string.Empty;
    private const string FileToWatch = "list_of_names.csv";

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IMediator mediator)
    {
        _logger = logger;
        _configuration = configuration;
        _mediator = mediator;
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

    private async void ProcessFile(string filePath)
    {
        try
        {
            _logger.LogInformation("Processing file: {FilePath}", filePath);
            
            // Add a short delay to ensure file is fully written
            await Task.Delay(500);
            
            // Use the ProcessNamesFile feature to process the file
            var result = await _mediator.Send(new ProcessNamesFile.Command(filePath));
            
            _logger.LogInformation("Processed {Count} names from {FilePath}. Results saved to {OutputPath}",
                result.ProcessedCount, filePath, result.OutputFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {FilePath}", filePath);
        }
    }
}
