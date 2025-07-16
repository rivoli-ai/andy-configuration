using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Integration;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceCollection _services;
    private readonly IConfigurationRoot _configuration;

    public ServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();
        _configuration = CreateConfiguration();
    }

    [Fact]
    public void AddAndyConfiguration_RegistersMainOptions()
    {
        // Act
        _services.AddAndyConfiguration(_configuration);
        var provider = _services.BuildServiceProvider();

        // Assert
        var options = provider.GetService<IOptions<AndyOptions>>();
        options.Should().NotBeNull();
        options!.Value.Should().NotBeNull();
        options.Value.Model.Name.Should().Be("test-model");
    }

    [Fact]
    public void AddAndyConfiguration_RegistersIndividualOptions()
    {
        // Act
        _services.AddAndyConfiguration(_configuration);
        var provider = _services.BuildServiceProvider();

        // Assert
        var modelOptions = provider.GetService<IOptions<ModelOptions>>();
        modelOptions.Should().NotBeNull();
        modelOptions!.Value.Name.Should().Be("test-model");

        var authOptions = provider.GetService<IOptions<AuthenticationOptions>>();
        authOptions.Should().NotBeNull();
        authOptions!.Value.Method.Should().Be(AuthenticationMethod.ApiKey);

        var uiOptions = provider.GetService<IOptions<UIOptions>>();
        uiOptions.Should().NotBeNull();
        uiOptions!.Value.Theme.Should().Be("dark");

        var toolOptions = provider.GetService<IOptions<ToolOptions>>();
        toolOptions.Should().NotBeNull();
        toolOptions!.Value.RequireConfirmation.Should().BeFalse();

        var loggingOptions = provider.GetService<IOptions<LoggingOptions>>();
        loggingOptions.Should().NotBeNull();
        loggingOptions!.Value.MinimumLevel.Should().Be("Debug");
    }

    [Fact]
    public void AddAndyConfiguration_RegistersValidators()
    {
        // Act
        _services.AddAndyConfiguration(_configuration);
        var provider = _services.BuildServiceProvider();

        // Assert
        var mainValidators = provider.GetServices<IValidateOptions<AndyOptions>>();
        mainValidators.Should().NotBeNull();
        mainValidators.Should().Contain(v => v.GetType() == typeof(ConfigurationValidator<AndyOptions>));

        var modelValidators = provider.GetServices<IValidateOptions<ModelOptions>>();
        modelValidators.Should().NotBeNull();
        modelValidators.Should().Contain(v => v.GetType() == typeof(ConfigurationValidator<ModelOptions>));
    }

    [Fact]
    public void AddAndyConfiguration_WithInvalidConfiguration_ThrowsOnStartup()
    {
        // Arrange
        var invalidConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Andy:Model:Name"] = "", // Required field is empty
                ["Andy:Model:Temperature"] = "3.0" // Out of range
            })
            .Build();

        // Act
        _services.AddAndyConfiguration(invalidConfig);
        var provider = _services.BuildServiceProvider();

        // Assert
        var action = () => provider.GetRequiredService<IOptions<AndyOptions>>().Value;
        action.Should().Throw<OptionsValidationException>();
    }

    [Fact]
    public void ValidateConfigurationOnStartup_RegistersHostedService()
    {
        // Act
        _services.AddAndyConfiguration(_configuration);
        _services.ValidateConfigurationOnStartup();

        // Assert
        var descriptor = _services.FirstOrDefault(d => 
            d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService) &&
            d.ImplementationType == typeof(ConfigurationValidationService));
        
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    private IConfigurationRoot CreateConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            ["Andy:Model:Name"] = "test-model",
            ["Andy:Model:Temperature"] = "0.8",
            ["Andy:Model:MaxTokens"] = "1000",
            
            ["Andy:Authentication:Method"] = "ApiKey",
            ["Andy:Authentication:ApiKey"] = "test-key",
            ["Andy:Authentication:CacheTokens"] = "true",
            ["Andy:Authentication:DefaultProvider"] = "TestProvider",
            
            ["Andy:UI:Theme"] = "dark",
            ["Andy:UI:EnableSyntaxHighlighting"] = "true",
            ["Andy:UI:EnableAutoScroll"] = "false",
            ["Andy:UI:MaxDisplayMessages"] = "50",
            
            ["Andy:Tools:RequireConfirmation"] = "false",
            ["Andy:Tools:DefaultTimeoutMs"] = "60000",
            ["Andy:Tools:MaxFileSizeBytes"] = "5242880",
            
            ["Andy:Logging:MinimumLevel"] = "Debug",
            ["Andy:Logging:WriteToFile"] = "true",
            ["Andy:Logging:FilePath"] = "logs/test.log",
            ["Andy:Logging:WriteToConsole"] = "false",
            ["Andy:Logging:ShowUserFriendlyApiMessages"] = "true"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }
}