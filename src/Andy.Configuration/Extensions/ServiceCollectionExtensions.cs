using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Andy.Configuration;

/// <summary>
/// Extension methods for adding configuration services to the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Andy configuration services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAndyConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the main configuration with validation
        services.Configure<AndyOptions>(configuration.GetSection(AndyOptions.SectionName));
        services.AddSingleton<IValidateOptions<AndyOptions>, ConfigurationValidator<AndyOptions>>();

        // Register individual option sections for direct injection
        services.Configure<ModelOptions>(configuration.GetSection($"{AndyOptions.SectionName}:Model"));
        services.AddSingleton<IValidateOptions<ModelOptions>, ConfigurationValidator<ModelOptions>>();

        services.Configure<AuthenticationOptions>(configuration.GetSection($"{AndyOptions.SectionName}:Authentication"));
        services.AddSingleton<IValidateOptions<AuthenticationOptions>, ConfigurationValidator<AuthenticationOptions>>();

        services.Configure<UIOptions>(configuration.GetSection($"{AndyOptions.SectionName}:UI"));
        services.AddSingleton<IValidateOptions<UIOptions>, ConfigurationValidator<UIOptions>>();

        services.Configure<ToolOptions>(configuration.GetSection($"{AndyOptions.SectionName}:Tools"));
        services.AddSingleton<IValidateOptions<ToolOptions>, ConfigurationValidator<ToolOptions>>();

        services.Configure<LoggingOptions>(configuration.GetSection($"{AndyOptions.SectionName}:Logging"));
        services.AddSingleton<IValidateOptions<LoggingOptions>, ConfigurationValidator<LoggingOptions>>();

        // Enable validation on startup
        services.AddOptions<AndyOptions>()
            .Bind(configuration.GetSection(AndyOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Validates configuration during application startup.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection ValidateConfigurationOnStartup(this IServiceCollection services)
    {
        // This will trigger validation on application startup
        services.AddHostedService<ConfigurationValidationService>();

        return services;
    }
}
