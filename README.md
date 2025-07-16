# Andy.Configuration

A robust, type-safe configuration library for .NET applications with built-in validation, dependency injection support, and comprehensive error handling.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)
[![Coverage](https://img.shields.io/badge/Coverage-96.94%25-brightgreen.svg)](https://github.com/rivoli-ai/andy-configuration)

## Features

- **Type-safe configuration** with strongly-typed option classes
- **Recursive validation** using Data Annotations
- **Dependency injection integration** with Microsoft.Extensions.DependencyInjection
- **Configuration validation on startup** to catch errors early
- **Support for multiple configuration sources** (JSON, environment variables, etc.)
- **Hierarchical configuration structure** with nested options
- **Comprehensive test coverage** (96.94% line coverage)

## Installation

```bash
dotnet add package Andy.Configuration
```

## Quick Start

### 1. Define Your Configuration

```csharp
public class MyAppOptions
{
    [Required]
    public DatabaseOptions Database { get; set; } = new();
    
    [Required]
    public ApiOptions Api { get; set; } = new();
}

public class DatabaseOptions
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
    
    [Range(1, 100)]
    public int MaxConnections { get; set; } = 10;
}

public class ApiOptions
{
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;
    
    [Range(1000, 300000)]
    public int TimeoutMs { get; set; } = 30000;
}
```

### 2. Configure in Startup

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add configuration services
builder.Services.AddAndyConfiguration(builder.Configuration);

// Enable validation on startup
builder.Services.ValidateConfigurationOnStartup();

var app = builder.Build();
```

### 3. Use in Your Application

```csharp
public class MyService
{
    private readonly MyAppOptions _options;
    
    public MyService(IOptions<MyAppOptions> options)
    {
        _options = options.Value;
    }
    
    public async Task ConnectToDatabase()
    {
        // Use strongly-typed configuration
        var connectionString = _options.Database.ConnectionString;
        var maxConnections = _options.Database.MaxConnections;
        // ...
    }
}
```

### 4. Configuration File (appsettings.json)

```json
{
  "MyApp": {
    "Database": {
      "ConnectionString": "Server=localhost;Database=myapp;",
      "MaxConnections": 20
    },
    "Api": {
      "BaseUrl": "https://api.example.com",
      "TimeoutMs": 60000
    }
  }
}
```

## Andy CLI Configuration

This library was originally built for the Andy CLI tool and includes pre-defined configuration options:

```csharp
// Use the built-in Andy configuration
builder.Services.AddAndyConfiguration(builder.Configuration);

// Access Andy options
public class MyAndyService
{
    private readonly AndyOptions _andyOptions;
    
    public MyAndyService(IOptions<AndyOptions> options)
    {
        _andyOptions = options.Value;
        
        // Access nested configuration
        var model = _andyOptions.Model.Name;
        var temperature = _andyOptions.Model.Temperature;
        var authMethod = _andyOptions.Authentication.Method;
    }
}
```

## Documentation

- [Design Documentation](docs/design.md) - Architecture and design decisions
- [API Reference](docs/api-reference.md) - Detailed API documentation
- [Configuration Guide](docs/configuration-guide.md) - Configuration options and examples
- [Contributing](docs/contributing.md) - Guidelines for contributors

## Key Components

### Configuration Validation

The library provides recursive validation of configuration objects:

- Required field validation
- Range validation for numeric values
- Custom validation attributes
- Nested object validation
- Collection validation (for required collections)

### Dependency Injection

Seamless integration with Microsoft.Extensions.DependencyInjection:

- Automatic registration of options and validators
- Support for `IOptions<T>`, `IOptionsSnapshot<T>`, and `IOptionsMonitor<T>`
- Individual option section registration for granular access

### Error Handling

Comprehensive error handling with detailed error messages:

- Validation errors are collected and reported with full property paths
- Startup validation prevents application start with invalid configuration
- Clear error messages for troubleshooting

## Testing

The library includes comprehensive unit and integration tests:

```bash
# Run tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate coverage report
reportgenerator -reports:"./TestResults/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
```

## Contributing

We welcome contributions! Please see our [Contributing Guide](docs/contributing.md) for details.

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Support

For issues, feature requests, or questions:
- Open an issue on [GitHub](https://github.com/rivoli-ai/andy-configuration/issues)
- Check the [documentation](docs/) for detailed guides

## Acknowledgments

Built with ❤️ using:
- [.NET 8](https://dotnet.microsoft.com)
- [Microsoft.Extensions.Options](https://www.nuget.org/packages/Microsoft.Extensions.Options/)
- [xUnit](https://xunit.net/) for testing
- [FluentAssertions](https://fluentassertions.com/) for test assertions
- [Coverlet](https://github.com/coverlet-coverage/coverlet) for code coverage