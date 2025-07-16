using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Andy.Configuration;

Console.WriteLine("=== Environment Variables Configuration Example ===\n");

// Show how to set environment variables
Console.WriteLine("This example demonstrates loading configuration from environment variables.");
Console.WriteLine("You can override any configuration value using environment variables.\n");

Console.WriteLine("Example environment variables (set these before running):");
Console.WriteLine("  export Andy__Model__Name=gpt-4-turbo");
Console.WriteLine("  export Andy__Model__Temperature=0.9");
Console.WriteLine("  export Andy__Authentication__ApiKey=your-actual-api-key");
Console.WriteLine("  export Andy__UI__Theme=dark");
Console.WriteLine();

// Set some example environment variables programmatically for demonstration
Environment.SetEnvironmentVariable("Andy__Model__Name", "claude-3-sonnet");
Environment.SetEnvironmentVariable("Andy__Model__Temperature", "0.9");
Environment.SetEnvironmentVariable("Andy__Authentication__ApiKey", "env-var-api-key-12345");
Environment.SetEnvironmentVariable("Andy__UI__Theme", "dark");

var builder = Host.CreateApplicationBuilder(args);

// Configure to load from multiple sources (order matters!)
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables(); // This will override JSON settings

// Add Andy configuration
builder.Services.AddAndyConfiguration(builder.Configuration);

// Enable validation
builder.Services.ValidateConfigurationOnStartup();

var host = builder.Build();

// Get the configuration
var andyOptions = host.Services.GetRequiredService<IOptions<AndyOptions>>().Value;

Console.WriteLine("Loaded Configuration (JSON + Environment Variables):");
Console.WriteLine("=" + new string('=', 50));

// Show which values came from where
Console.WriteLine("\nModel Configuration:");
Console.WriteLine($"  Name: {andyOptions.Model.Name} (from: ENVIRONMENT VARIABLE)");
Console.WriteLine($"  Temperature: {andyOptions.Model.Temperature} (from: ENVIRONMENT VARIABLE)");
Console.WriteLine($"  Max Tokens: {andyOptions.Model.MaxTokens?.ToString() ?? "Not set"} (from: appsettings.json)");

Console.WriteLine("\nAuthentication:");
Console.WriteLine($"  Method: {andyOptions.Authentication.Method} (from: appsettings.json)");
Console.WriteLine($"  API Key: ***{andyOptions.Authentication.ApiKey?[^Math.Min(5, andyOptions.Authentication.ApiKey?.Length ?? 0)..] ?? ""} (from: ENVIRONMENT VARIABLE)");

Console.WriteLine("\nUI Configuration:");
Console.WriteLine($"  Theme: {andyOptions.UI.Theme} (from: ENVIRONMENT VARIABLE)");
Console.WriteLine($"  Enable Syntax Highlighting: {andyOptions.UI.EnableSyntaxHighlighting} (from: appsettings.json)");

Console.WriteLine("\nLogging:");
Console.WriteLine($"  Minimum Level: {andyOptions.Logging.MinimumLevel} (from: appsettings.json)");
Console.WriteLine($"  Write to File: {andyOptions.Logging.WriteToFile} (from: appsettings.json)");

// Demonstrate configuration priority
Console.WriteLine("\n\nConfiguration Priority (highest to lowest):");
Console.WriteLine("1. Command line arguments");
Console.WriteLine("2. Environment variables");
Console.WriteLine("3. appsettings.{Environment}.json");
Console.WriteLine("4. appsettings.json");
Console.WriteLine("5. Default values in code");

// Show all Andy-related environment variables
Console.WriteLine("\n\nCurrent Andy-related environment variables:");
var envVars = Environment.GetEnvironmentVariables();
foreach (var key in envVars.Keys)
{
    var keyStr = key.ToString();
    if (keyStr != null && keyStr.StartsWith("Andy__"))
    {
        Console.WriteLine($"  {keyStr} = {envVars[key]}");
    }
}

// Clean up the environment variables we set
Environment.SetEnvironmentVariable("Andy__Model__Name", null);
Environment.SetEnvironmentVariable("Andy__Model__Temperature", null);
Environment.SetEnvironmentVariable("Andy__Authentication__ApiKey", null);
Environment.SetEnvironmentVariable("Andy__UI__Theme", null);

Console.WriteLine("\n✅ Environment variables configuration example completed!");