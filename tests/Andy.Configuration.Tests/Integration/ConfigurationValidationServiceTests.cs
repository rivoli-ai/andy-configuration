using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Integration;

public class ConfigurationValidationServiceTests
{
    private readonly Mock<ILogger<ConfigurationValidationService>> _loggerMock;
    private readonly IServiceCollection _services;

    public ConfigurationValidationServiceTests()
    {
        _loggerMock = new Mock<ILogger<ConfigurationValidationService>>();
        _services = new ServiceCollection();
    }

    [Fact]
    public async Task StartAsync_WithValidConfiguration_LogsSuccess()
    {
        // Arrange
        var config = CreateValidConfiguration();
        _services.AddSingleton<IConfiguration>(config);
        _services.AddAndyConfiguration(config);
        _services.AddSingleton(_loggerMock.Object);
        
        var provider = _services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<AndyOptions>>();
        var service = new ConfigurationValidationService(_loggerMock.Object, optionsMonitor);

        // Act
        await service.StartAsync(CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Configuration validation completed successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_WithInvalidConfiguration_ThrowsAndLogsError()
    {
        // Arrange
        var config = CreateInvalidConfiguration();
        _services.AddSingleton<IConfiguration>(config);
        _services.AddAndyConfiguration(config);
        _services.AddSingleton(_loggerMock.Object);
        
        var provider = _services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<AndyOptions>>();
        var service = new ConfigurationValidationService(_loggerMock.Object, optionsMonitor);

        // Act
        var act = () => service.StartAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OptionsValidationException>();
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Configuration validation failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_LogsConfigurationSummary()
    {
        // Arrange
        var config = CreateValidConfiguration();
        _services.AddSingleton<IConfiguration>(config);
        _services.AddAndyConfiguration(config);
        _services.AddSingleton(_loggerMock.Object);
        
        var provider = _services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<AndyOptions>>();
        var service = new ConfigurationValidationService(_loggerMock.Object, optionsMonitor);

        // Act
        await service.StartAsync(CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Using model: test-model with temperature: 0.8")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Authentication method: ApiKey")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("UI theme: dark")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StopAsync_CompletesSuccessfully()
    {
        // Arrange
        var config = CreateValidConfiguration();
        _services.AddSingleton<IConfiguration>(config);
        _services.AddAndyConfiguration(config);
        
        var provider = _services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<AndyOptions>>();
        var service = new ConfigurationValidationService(_loggerMock.Object, optionsMonitor);

        // Act
        await service.StopAsync(CancellationToken.None);

        // Assert
        // StopAsync should complete without any issues
    }

    [Fact]
    public async Task HostedService_Integration_ValidatesOnStartup()
    {
        // Arrange
        var config = CreateValidConfiguration();
        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConfiguration>(config);
                services.AddAndyConfiguration(config);
                services.ValidateConfigurationOnStartup();
            })
            .ConfigureLogging(logging => logging.ClearProviders());

        // Act & Assert
        using var host = hostBuilder.Build();
        var act = () => host.StartAsync();
        await act.Should().NotThrowAsync();
    }

    private IConfiguration CreateValidConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            ["Andy:Model:Name"] = "test-model",
            ["Andy:Model:Temperature"] = "0.8",
            ["Andy:Authentication:Method"] = "ApiKey",
            ["Andy:Authentication:ApiKey"] = "test-key",
            ["Andy:UI:Theme"] = "dark",
            ["Andy:Tools:RequireConfirmation"] = "false",
            ["Andy:Logging:MinimumLevel"] = "Information"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    private IConfiguration CreateInvalidConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            ["Andy:Model:Name"] = "", // Required field is empty
            ["Andy:Model:Temperature"] = "3.0", // Out of range
            ["Andy:Authentication:Method"] = "ApiKey", // Valid enum value
            ["Andy:UI:Theme"] = "", // Required field is empty
            ["Andy:Tools:DefaultTimeoutMs"] = "-1000", // Negative value
            ["Andy:Logging:MinimumLevel"] = ""
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }
}