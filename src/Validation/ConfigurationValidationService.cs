using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Andy.Configuration;

/// <summary>
/// Hosted service that validates configuration on application startup.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConfigurationValidationService"/> class.
/// </remarks>
/// <param name="logger">The logger.</param>
/// <param name="options">The options monitor.</param>
public class ConfigurationValidationService(
    ILogger<ConfigurationValidationService> logger,
    IOptionsMonitor<AndyOptions> options) : IHostedService
{
    private readonly ILogger<ConfigurationValidationService> _logger = logger;
    private readonly IOptionsMonitor<AndyOptions> _options = options;

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // This will trigger validation and throw if invalid
            var config = _options.CurrentValue;
            _logger.LogInformation("Configuration validation completed successfully");

            // Log configuration summary (without sensitive data)
            _logger.LogDebug("Using model: {ModelName} with temperature: {Temperature}",
                config.Model.Name, config.Model.Temperature);
            _logger.LogDebug("Authentication method: {AuthMethod}", config.Authentication.Method);
            _logger.LogDebug("UI theme: {Theme}", config.UI.Theme);
        }
        catch (OptionsValidationException ex)
        {
            _logger.LogError(ex, "Configuration validation failed: {Failures}",
                string.Join(", ", ex.Failures));
            throw;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
