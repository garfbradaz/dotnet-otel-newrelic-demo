using NameGenderizer.Service;
using NameGenderizer.Service.Services;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add services
builder.Services.AddHttpClient<INameApiClient, NameApiClient>();
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
