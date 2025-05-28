using Microsoft.Extensions.Logging;

namespace NameGenderizer.Service.Extensions;

public static partial class LoggerMessageExtensions
{
    // CSV Service log definitions
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Reading CSV file: {FilePath}")]
    public static partial void LogReadingCsvFile(this ILogger logger, string filePath);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Read {Count} records from CSV file")]
    public static partial void LogReadCsvRecordsCount(this ILogger logger, int count);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Error reading CSV file: {FilePath}")]
    public static partial void LogReadCsvError(this ILogger logger, Exception exception, string filePath);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Information,
        Message = "Writing {Count} records to CSV file: {FilePath}")]
    public static partial void LogWritingCsvFile(this ILogger logger, int count, string filePath);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "Successfully wrote records to CSV file")]
    public static partial void LogWroteCsvFile(this ILogger logger);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Error,
        Message = "Error writing CSV file: {FilePath}")]
    public static partial void LogWriteCsvError(this ILogger logger, Exception exception, string filePath);

    // NameApiClient log definitions
    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Error,
        Message = "Failed to deserialize API response")]
    public static partial void LogDeserializeError(this ILogger logger);
    
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Error,
        Message = "Error calling Name API for {FirstName} {LastName}")]
    public static partial void LogNameApiError(this ILogger logger, Exception exception, string firstName, string lastName);

    // Worker log definitions
    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Information,
        Message = "Starting service. Watching directory: {Directory}")]
    public static partial void LogStartingService(this ILogger logger, string directory);

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Information,
        Message = "Created watch directory: {Directory}")]
    public static partial void LogCreatedWatchDirectory(this ILogger logger, string directory);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Information,
        Message = "Worker started at: {time}")]
    public static partial void LogWorkerStarted(this ILogger logger, DateTimeOffset time);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Information,
        Message = "File watcher set up for {FileName} in {Directory}")]
    public static partial void LogFileWatcherSetup(this ILogger logger, string fileName, string directory);

    [LoggerMessage(
        EventId = 3004,
        Level = LogLevel.Information,
        Message = "Stopping service")]
    public static partial void LogStoppingService(this ILogger logger);
    
    [LoggerMessage(
        EventId = 3005,
        Level = LogLevel.Information,
        Message = "Processing file: {FilePath}")]
    public static partial void LogProcessingFile(this ILogger logger, string filePath);

    [LoggerMessage(
        EventId = 3006,
        Level = LogLevel.Error,
        Message = "Error getting gender for {FirstName} {LastName}")]
    public static partial void LogGettingGenderError(this ILogger logger, Exception exception, string firstName, string lastName);

    [LoggerMessage(
        EventId = 3007,
        Level = LogLevel.Information,
        Message = "File {FilePath} processed successfully. Output saved to {OutputPath}")]
    public static partial void LogFileProcessedSuccessfully(this ILogger logger, string filePath, string outputPath);

    [LoggerMessage(
        EventId = 3008,
        Level = LogLevel.Error,
        Message = "Error processing file {FilePath}")]
    public static partial void LogProcessingFileError(this ILogger logger, Exception exception, string filePath);
}