using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Andy.Configuration;
using System.ComponentModel.DataAnnotations;

var commandLineArgs = Environment.GetCommandLineArgs();
var useInvalidConfig = commandLineArgs.Contains("--invalid");

var builder = Host.CreateApplicationBuilder(args);

// Clear existing configuration sources
builder.Configuration.Sources.Clear();

// Add configuration file based on command line argument
if (useInvalidConfig)
{
    Console.WriteLine("=== Using INVALID configuration file ===");
    builder.Configuration.AddJsonFile("appsettings.invalid.json", optional: false);
}
else
{
    Console.WriteLine("=== Using VALID configuration file ===");
    builder.Configuration.AddJsonFile("appsettings.json", optional: false);
}

// Configure services
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection(AppOptions.SectionName));

// Add validation
builder.Services.AddSingleton<IValidateOptions<AppOptions>, ConfigurationValidator<AppOptions>>();

// Enable validation on startup
builder.Services.ValidateConfigurationOnStartup();

try
{
    var host = builder.Build();
    
    // If we get here, validation passed
    Console.WriteLine("\n✅ Configuration validation PASSED!");
    
    var options = host.Services.GetRequiredService<IOptions<AppOptions>>().Value;
    
    Console.WriteLine("\nLoaded configuration:");
    Console.WriteLine($"  App Name: {options.Name}");
    Console.WriteLine($"  Database:");
    Console.WriteLine($"    Connection: {options.Database.ConnectionString}");
    Console.WriteLine($"    Pool Size: {options.Database.PoolSize}");
    Console.WriteLine($"    Timeout: {options.Database.TimeoutMs}ms");
    Console.WriteLine($"  API:");
    Console.WriteLine($"    Base URL: {options.Api.BaseUrl}");
    Console.WriteLine($"    API Key: ***{options.Api.ApiKey[^4..]}");
    Console.WriteLine($"    Retry Count: {options.Api.RetryCount}");
}
catch (OptionsValidationException ex)
{
    Console.WriteLine("\n❌ Configuration validation FAILED!");
    Console.WriteLine("\nValidation errors:");
    
    foreach (var failure in ex.Failures)
    {
        Console.WriteLine($"  - {failure}");
    }
    
    Console.WriteLine("\nTip: Run without --invalid flag to see a valid configuration");
    return 1;
}

Console.WriteLine("\nTip: Run with --invalid flag to see validation errors in action");
return 0;

// Example of custom configuration with validation
public class AppOptions
{
    public const string SectionName = "App";

    [Required(ErrorMessage = "Application name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Database configuration is required")]
    public DatabaseOptions Database { get; set; } = new();

    [Required(ErrorMessage = "API configuration is required")]
    public ApiOptions Api { get; set; } = new();
}

public class DatabaseOptions
{
    [Required(ErrorMessage = "Connection string is required")]
    [MinLength(10, ErrorMessage = "Connection string seems too short")]
    public string ConnectionString { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "Connection pool size must be between 1 and 100")]
    public int PoolSize { get; set; } = 10;

    [Range(1000, 60000, ErrorMessage = "Timeout must be between 1 and 60 seconds")]
    public int TimeoutMs { get; set; } = 30000;
}

public class ApiOptions
{
    [Required(ErrorMessage = "API base URL is required")]
    [Url(ErrorMessage = "Must be a valid URL")]
    public string BaseUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "API key is required")]
    [RegularExpression(@"^[A-Za-z0-9\-_]{20,}$", ErrorMessage = "API key must be at least 20 characters of alphanumeric, dash, or underscore")]
    public string ApiKey { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Retry count must be between 1 and 10")]
    public int RetryCount { get; set; } = 3;
}

