using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Andy.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration
builder.Services.AddAndyConfiguration(builder.Configuration);

// Build the host
var host = builder.Build();

// Demonstrate accessing configuration
var andyOptions = host.Services.GetRequiredService<IOptions<AndyOptions>>().Value;

Console.WriteLine("=== Andy Configuration Example ===");
Console.WriteLine();

// Display model configuration
Console.WriteLine("Model Configuration:");
Console.WriteLine($"  Name: {andyOptions.Model.Name}");
Console.WriteLine($"  Temperature: {andyOptions.Model.Temperature}");
Console.WriteLine($"  Max Tokens: {andyOptions.Model.MaxTokens?.ToString() ?? "Not set"}");
Console.WriteLine($"  Top P: {andyOptions.Model.TopP?.ToString() ?? "Not set"}");
Console.WriteLine($"  Top K: {andyOptions.Model.TopK?.ToString() ?? "Not set"}");
Console.WriteLine();

// Display authentication configuration
Console.WriteLine("Authentication Configuration:");
Console.WriteLine($"  Method: {andyOptions.Authentication.Method}");
if (andyOptions.Authentication.Method == AuthenticationMethod.ApiKey)
{
    Console.WriteLine($"  API Key: {(string.IsNullOrEmpty(andyOptions.Authentication.ApiKey) ? "Not Set" : "***" + andyOptions.Authentication.ApiKey[^4..])}");
}
Console.WriteLine();

// Display UI configuration
Console.WriteLine("UI Configuration:");
Console.WriteLine($"  Theme: {andyOptions.UI.Theme}");
Console.WriteLine($"  Enable Syntax Highlighting: {andyOptions.UI.EnableSyntaxHighlighting}");
Console.WriteLine($"  Enable Auto Scroll: {andyOptions.UI.EnableAutoScroll}");
Console.WriteLine($"  Max Display Messages: {andyOptions.UI.MaxDisplayMessages}");
Console.WriteLine();

// Display Tool configuration
Console.WriteLine("Tool Configuration:");
Console.WriteLine($"  Require Confirmation: {andyOptions.Tools.RequireConfirmation}");
Console.WriteLine($"  Default Timeout: {andyOptions.Tools.DefaultTimeoutMs}ms");
Console.WriteLine($"  Max File Size: {andyOptions.Tools.MaxFileSizeBytes / 1024 / 1024}MB");
Console.WriteLine();

// Display Logging configuration
Console.WriteLine("Logging Configuration:");
Console.WriteLine($"  Minimum Level: {andyOptions.Logging.MinimumLevel}");
Console.WriteLine($"  Write to File: {andyOptions.Logging.WriteToFile}");
Console.WriteLine($"  File Path: {andyOptions.Logging.FilePath}");
Console.WriteLine($"  Write to Console: {andyOptions.Logging.WriteToConsole}");
Console.WriteLine();

// Demonstrate configuration monitoring
var optionsMonitor = host.Services.GetRequiredService<IOptionsMonitor<AndyOptions>>();
optionsMonitor.OnChange((options, name) =>
{
    Console.WriteLine($"Configuration changed! New model temperature: {options.Model.Temperature}");
});

Console.WriteLine("Press any key to exit...");
Console.ReadKey();