using NameGenderizer.Service;
using NameGenderizer.Service.Services;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add MediatR
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// Add HTTP client for the name genderizer API
builder.Services.AddHttpClient<NameGenderizerApiClient>();
builder.Services.AddTransient<NameGenderizerApiClient>();

// Add services
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
