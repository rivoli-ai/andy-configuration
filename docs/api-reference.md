# API Reference

## Table of Contents

- [Option Classes](#option-classes)
- [Extension Methods](#extension-methods)
- [Validation Classes](#validation-classes)
- [Services](#services)

## Option Classes

### AndyOptions

Root configuration class for Andy CLI.

```csharp
public class AndyOptions
{
    public const string SectionName = "Andy";
    
    [Required]
    public ModelOptions Model { get; set; } = new();
    
    [Required]
    public AuthenticationOptions Authentication { get; set; } = new();
    
    [Required]
    public UIOptions UI { get; set; } = new();
    
    [Required]
    public ToolOptions Tools { get; set; } = new();
    
    [Required]
    public LoggingOptions Logging { get; set; } = new();
}
```

### ModelOptions

Configuration for AI model settings.

```csharp
public class ModelOptions
{
    [Required]
    public string Name { get; set; } = "gemini-2.0-flash-exp";
    
    [Range(0.0, 2.0)]
    public double Temperature { get; set; } = 0.7;
    
    [Range(1, 32768)]
    public int? MaxTokens { get; set; }
    
    [Range(0.0, 1.0)]
    public double? TopP { get; set; }
    
    [Range(1, 100)]
    public int? TopK { get; set; }
}
```

### AuthenticationOptions

Configuration for authentication settings.

```csharp
public class AuthenticationOptions
{
    [Required]
    public AuthenticationMethod Method { get; set; } = AuthenticationMethod.OAuth;
    
    public bool CacheTokens { get; set; } = true;
    
    public string? ApiKey { get; set; }
    
    public OAuthOptions OAuth { get; set; } = new();
    
    public VertexAIOptions VertexAI { get; set; } = new();
    
    public string DefaultProvider { get; set; } = "OpenAI";
}
```

### AuthenticationMethod

Enumeration of supported authentication methods.

```csharp
public enum AuthenticationMethod
{
    OAuth,
    ApiKey,
    VertexAI
}
```

### OAuthOptions

OAuth-specific configuration.

```csharp
public class OAuthOptions
{
    public string? ClientId { get; set; }
    
    public string? ClientSecret { get; set; }
    
    public string RedirectUri { get; set; } = "http://localhost:8080/callback";
}
```

### VertexAIOptions

Google Vertex AI configuration.

```csharp
public class VertexAIOptions
{
    public string? ProjectId { get; set; }
    
    public string Region { get; set; } = "us-central1";
    
    public string? ServiceAccountKeyPath { get; set; }
}
```

### UIOptions

User interface configuration.

```csharp
public class UIOptions
{
    [Required]
    public string Theme { get; set; } = "default";
    
    public bool EnableSyntaxHighlighting { get; set; } = true;
    
    public bool EnableAutoScroll { get; set; } = true;
    
    [Range(10, 1000)]
    public int MaxDisplayMessages { get; set; } = 100;
}
```

### ToolOptions

Tool execution configuration.

```csharp
public class ToolOptions
{
    public bool RequireConfirmation { get; set; } = true;
    
    [Range(1000, 300000)]
    public int DefaultTimeoutMs { get; set; } = 30000;
    
    [Range(1024, 100 * 1024 * 1024)]
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
}
```

### LoggingOptions

Logging configuration.

```csharp
public class LoggingOptions
{
    [Required]
    public string MinimumLevel { get; set; } = "Information";
    
    public bool WriteToFile { get; set; } = true;
    
    public string FilePath { get; set; } = "logs/andy-.log";
    
    public bool WriteToConsole { get; set; } = true;
    
    public bool ShowUserFriendlyApiMessages { get; set; } = true;
}
```

## Extension Methods

### ServiceCollectionExtensions

Extension methods for configuring dependency injection.

#### AddAndyConfiguration

Registers Andy configuration services with the DI container.

```csharp
public static IServiceCollection AddAndyConfiguration(
    this IServiceCollection services, 
    IConfiguration configuration)
```

**Parameters:**
- `services`: The service collection to configure
- `configuration`: The configuration instance

**Returns:** The service collection for chaining

**Example:**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAndyConfiguration(builder.Configuration);
```

#### ValidateConfigurationOnStartup

Enables configuration validation during application startup.

```csharp
public static IServiceCollection ValidateConfigurationOnStartup(
    this IServiceCollection services)
```

**Parameters:**
- `services`: The service collection to configure

**Returns:** The service collection for chaining

**Example:**
```csharp
builder.Services.AddAndyConfiguration(builder.Configuration)
                .ValidateConfigurationOnStartup();
```

## Validation Classes

### ConfigurationValidator<T>

Generic validator for configuration objects using Data Annotations.

```csharp
public class ConfigurationValidator<T> : IValidateOptions<T> where T : class
{
    public ValidateOptionsResult Validate(string? name, T options)
}
```

**Type Parameters:**
- `T`: The type of options to validate

**Methods:**

#### Validate

Validates a configuration object and all its properties recursively.

**Parameters:**
- `name`: The name of the options instance being validated
- `options`: The options instance to validate

**Returns:** `ValidateOptionsResult` indicating success or failure with error messages

### ValidationExtensions

Static helper methods for recursive validation.

#### TryValidateObjectRecursively

Validates an object and all its properties recursively using Data Annotations.

```csharp
public static bool TryValidateObjectRecursively(
    object obj, 
    ValidationContext validationContext, 
    ICollection<ValidationResult> validationResults, 
    bool validateAllProperties)
```

**Parameters:**
- `obj`: The object to validate
- `validationContext`: The validation context
- `validationResults`: Collection to store validation results
- `validateAllProperties`: Whether to validate all properties

**Returns:** `true` if validation passes, `false` otherwise

**Note:** This method validates nested objects but does not validate items within collections by default.

## Services

### ConfigurationValidationService

Hosted service that validates configuration on application startup.

```csharp
public class ConfigurationValidationService : IHostedService
{
    public ConfigurationValidationService(
        ILogger<ConfigurationValidationService> logger,
        IOptionsMonitor<AndyOptions> options)
    
    public Task StartAsync(CancellationToken cancellationToken)
    
    public Task StopAsync(CancellationToken cancellationToken)
}
```

**Constructor Parameters:**
- `logger`: Logger for validation messages
- `options`: Options monitor for accessing configuration

**Methods:**

#### StartAsync

Validates configuration when the service starts.

**Parameters:**
- `cancellationToken`: Cancellation token

**Returns:** Completed task

**Throws:** `OptionsValidationException` if validation fails

#### StopAsync

Called when the service stops (no-op).

**Parameters:**
- `cancellationToken`: Cancellation token

**Returns:** Completed task

## Usage Examples

### Basic Configuration

```csharp
// appsettings.json
{
  "Andy": {
    "Model": {
      "Name": "gpt-4",
      "Temperature": 0.8
    },
    "Authentication": {
      "Method": "ApiKey",
      "ApiKey": "your-api-key"
    },
    "UI": {
      "Theme": "dark"
    },
    "Tools": {
      "RequireConfirmation": false
    },
    "Logging": {
      "MinimumLevel": "Debug"
    }
  }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAndyConfiguration(builder.Configuration)
                .ValidateConfigurationOnStartup();

var app = builder.Build();

// Usage in a service
public class MyService
{
    private readonly AndyOptions _options;
    
    public MyService(IOptions<AndyOptions> options)
    {
        _options = options.Value;
    }
}
```

### Custom Validation

```csharp
public class CustomOptionsValidator : IValidateOptions<MyOptions>
{
    public ValidateOptionsResult Validate(string name, MyOptions options)
    {
        var failures = new List<string>();
        
        if (options.StartDate > options.EndDate)
        {
            failures.Add("StartDate must be before EndDate");
        }
        
        return failures.Any() 
            ? ValidateOptionsResult.Fail(failures) 
            : ValidateOptionsResult.Success;
    }
}

// Register custom validator
services.AddSingleton<IValidateOptions<MyOptions>, CustomOptionsValidator>();
```

### Accessing Nested Options

```csharp
public class ModelService
{
    private readonly ModelOptions _modelOptions;
    
    public ModelService(IOptions<ModelOptions> options)
    {
        _modelOptions = options.Value;
    }
    
    public async Task<string> GenerateResponse(string prompt)
    {
        var temperature = _modelOptions.Temperature;
        var maxTokens = _modelOptions.MaxTokens ?? 1000;
        // Use configuration values
    }
}
```

## Error Handling

### Validation Errors

Validation errors are aggregated and thrown as `OptionsValidationException`:

```csharp
try
{
    var options = serviceProvider.GetRequiredService<IOptions<AndyOptions>>().Value;
}
catch (OptionsValidationException ex)
{
    foreach (var failure in ex.Failures)
    {
        Console.WriteLine($"Validation failed: {failure}");
    }
}
```

### Configuration Binding Errors

Configuration binding errors result in `InvalidOperationException`:

```csharp
try
{
    var options = configuration.GetSection("Andy").Get<AndyOptions>();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Configuration binding failed: {ex.Message}");
}
```