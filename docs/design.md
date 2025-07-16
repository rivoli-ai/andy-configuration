# Design Documentation

## Overview

Andy.Configuration is a .NET library designed to provide a robust, type-safe configuration management system with built-in validation and dependency injection support. This document outlines the architectural decisions, design patterns, and key components of the library.

## Architecture

### Core Components

```
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
├─────────────────────────────────────────────────────────────┤
│                 Dependency Injection                        │
│             (ServiceCollectionExtensions)                   │
├─────────────────────────────────────────────────────────────┤
│                  Validation Layer                           │
│       (ConfigurationValidator, ValidationExtensions)        │
├─────────────────────────────────────────────────────────────┤
│               Configuration Options                         │
│                 (Option Classes)                            │
├─────────────────────────────────────────────────────────────┤
│             Configuration Sources                           │
│        (JSON, Environment Variables, etc.)                  │
└─────────────────────────────────────────────────────────────┘
```

### Key Design Patterns

#### 1. Options Pattern
The library leverages Microsoft's Options pattern to provide strongly-typed access to configuration data. This pattern offers:
- Type safety at compile time
- IntelliSense support in IDEs
- Clear separation of configuration concerns
- Support for configuration changes at runtime

#### 2. Validator Pattern
Custom validators implement `IValidateOptions<T>` to provide comprehensive validation:
- Recursive validation of nested objects
- Integration with Data Annotations
- Custom validation logic support
- Early validation during application startup

#### 3. Builder Pattern
Extension methods provide a fluent API for configuration setup:
```csharp
services.AddAndyConfiguration(configuration)
        .ValidateConfigurationOnStartup();
```

## Component Design

### Option Classes

Option classes are POCOs (Plain Old CLR Objects) that represent configuration sections:

```csharp
public class AndyOptions
{
    public const string SectionName = "Andy";
    
    [Required]
    public ModelOptions Model { get; set; } = new();
    
    [Required]
    public AuthenticationOptions Authentication { get; set; } = new();
}
```

**Design Decisions:**
- Default instances prevent null reference exceptions
- Data Annotations provide declarative validation
- Nested structure mirrors JSON configuration hierarchy
- Property initializers ensure valid default state

### Validation System

The validation system consists of two main components:

#### ConfigurationValidator<T>
- Generic validator for any configuration type
- Implements `IValidateOptions<T>` for DI integration
- Delegates to `ValidationExtensions` for recursive validation

#### ValidationExtensions
- Static helper class for recursive validation
- Handles complex types, collections, and primitives
- Builds comprehensive error messages with property paths

**Validation Flow:**
1. Root object validation using Data Annotations
2. Recursive descent into complex properties
3. Collection iteration (items not validated by default)
4. Error aggregation with full property paths

### Dependency Injection Integration

The `ServiceCollectionExtensions` class provides seamless DI integration:

```csharp
public static IServiceCollection AddAndyConfiguration(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // Register main options
    services.Configure<AndyOptions>(
        configuration.GetSection(AndyOptions.SectionName));
    
    // Register validator
    services.AddSingleton<IValidateOptions<AndyOptions>, 
        ConfigurationValidator<AndyOptions>>();
    
    // Register nested options for granular access
    services.Configure<ModelOptions>(
        configuration.GetSection($"{AndyOptions.SectionName}:Model"));
    
    // Enable validation on startup
    services.AddOptions<AndyOptions>()
        .Bind(configuration.GetSection(AndyOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();
    
    return services;
}
```

**Design Benefits:**
- Single method call configures everything
- Supports both aggregate and granular option access
- Automatic validator registration
- Fail-fast validation on startup

### Startup Validation

The `ConfigurationValidationService` implements `IHostedService` to validate configuration during application startup:

```csharp
public class ConfigurationValidationService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var config = _options.CurrentValue; // Triggers validation
            _logger.LogInformation("Configuration validation completed");
            LogConfigurationSummary(config);
        }
        catch (OptionsValidationException ex)
        {
            _logger.LogError(ex, "Configuration validation failed");
            throw; // Prevents application startup
        }
        return Task.CompletedTask;
    }
}
```

## Error Handling

### Validation Errors
- Collected during recursive validation
- Include full property path (e.g., "Model.Temperature")
- Aggregated into `OptionsValidationException`
- Logged with appropriate severity

### Configuration Binding Errors
- Handled by Microsoft.Extensions.Configuration
- Type conversion errors (e.g., invalid enum values)
- Missing required configuration sections
- Format errors in configuration sources

## Performance Considerations

### Validation Performance
- Validation occurs once at startup
- Reflection usage is minimized
- Property iteration skips indexed properties
- Collections are not deeply validated by default

### Memory Usage
- Option objects are singletons by default
- Validators are registered as singletons
- No unnecessary object allocations during validation

## Extensibility

### Custom Validators
Users can create custom validators by implementing `IValidateOptions<T>`:

```csharp
public class CustomValidator : IValidateOptions<MyOptions>
{
    public ValidateOptionsResult Validate(string name, MyOptions options)
    {
        // Custom validation logic
        if (options.SomeProperty > options.OtherProperty)
        {
            return ValidateOptionsResult.Fail(
                "SomeProperty must be less than OtherProperty");
        }
        return ValidateOptionsResult.Success;
    }
}
```

### Custom Option Classes
Any POCO can be used as an option class:
- Apply Data Annotations for validation
- Use property initializers for defaults
- Nest other option classes for hierarchy

## Security Considerations

### Sensitive Data
- No logging of sensitive configuration values
- Support for secret management systems
- Configuration values are not exposed in exceptions

### Validation Security
- Input validation prevents injection attacks
- Range validation prevents resource exhaustion
- Type safety prevents type confusion attacks

## Future Enhancements

### Planned Features
1. **Configuration Hot Reload** - Support for configuration changes without restart
2. **Async Validation** - Support for async validation scenarios
3. **Configuration Encryption** - Built-in encryption for sensitive values
4. **Validation Policies** - Configurable validation severity levels

### Extension Points
1. **Custom Configuration Sources** - Plugin architecture for custom sources
2. **Validation Middleware** - HTTP middleware for configuration validation
3. **Configuration UI** - Auto-generated configuration UI from option classes
4. **Schema Generation** - JSON Schema generation from option classes

## Conclusion

Andy.Configuration provides a solid foundation for configuration management in .NET applications. The design prioritizes:
- Type safety and compile-time checking
- Comprehensive validation with clear error messages
- Seamless integration with .NET's DI container
- Extensibility for custom scenarios

The library follows SOLID principles and established .NET patterns to ensure maintainability and ease of use.