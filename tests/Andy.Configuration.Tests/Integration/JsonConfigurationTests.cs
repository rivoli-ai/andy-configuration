using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Integration;

public class JsonConfigurationTests
{
    [Fact]
    public void LoadFromJsonFile_BindsAllPropertiesCorrectly()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("TestData/appsettings.test.json", optional: false, reloadOnChange: false)
            .Build();

        var services = new ServiceCollection();
        services.AddAndyConfiguration(configuration);
        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<AndyOptions>>().Value;

        // Assert
        // Model options
        options.Model.Should().NotBeNull();
        options.Model.Name.Should().Be("test-model-from-json");
        options.Model.Temperature.Should().Be(0.9);
        options.Model.MaxTokens.Should().Be(2048);
        options.Model.TopP.Should().Be(0.95);
        options.Model.TopK.Should().Be(40);

        // Authentication options
        options.Authentication.Should().NotBeNull();
        options.Authentication.Method.Should().Be(AuthenticationMethod.OAuth);
        options.Authentication.CacheTokens.Should().BeTrue();
        options.Authentication.DefaultProvider.Should().Be("TestProvider");
        
        // OAuth options
        options.Authentication.OAuth.Should().NotBeNull();
        options.Authentication.OAuth.ClientId.Should().Be("test-client-id");
        options.Authentication.OAuth.ClientSecret.Should().Be("test-client-secret");
        options.Authentication.OAuth.RedirectUri.Should().Be("http://localhost:9090/callback");
        
        // VertexAI options
        options.Authentication.VertexAI.Should().NotBeNull();
        options.Authentication.VertexAI.ProjectId.Should().Be("test-project");
        options.Authentication.VertexAI.Region.Should().Be("us-east1");
        options.Authentication.VertexAI.ServiceAccountKeyPath.Should().Be("/path/to/key.json");

        // UI options
        options.UI.Should().NotBeNull();
        options.UI.Theme.Should().Be("light");
        options.UI.EnableSyntaxHighlighting.Should().BeTrue();
        options.UI.EnableAutoScroll.Should().BeTrue();
        options.UI.MaxDisplayMessages.Should().Be(200);

        // Tools options
        options.Tools.Should().NotBeNull();
        options.Tools.RequireConfirmation.Should().BeTrue();
        options.Tools.DefaultTimeoutMs.Should().Be(45000);
        options.Tools.MaxFileSizeBytes.Should().Be(20971520);

        // Logging options
        options.Logging.Should().NotBeNull();
        options.Logging.MinimumLevel.Should().Be("Warning");
        options.Logging.WriteToFile.Should().BeTrue();
        options.Logging.FilePath.Should().Be("logs/test-andy.log");
        options.Logging.WriteToConsole.Should().BeTrue();
        options.Logging.ShowUserFriendlyApiMessages.Should().BeFalse();
    }

    [Fact]
    public void PartialJsonConfiguration_UsesDefaultsForMissingValues()
    {
        // Arrange
        var partialConfig = new Dictionary<string, string?>
        {
            ["Andy:Model:Name"] = "partial-model",
            ["Andy:Authentication:Method"] = "ApiKey",
            ["Andy:UI:Theme"] = "custom",
            ["Andy:Tools:RequireConfirmation"] = "true",
            ["Andy:Logging:MinimumLevel"] = "Debug"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(partialConfig)
            .Build();

        var services = new ServiceCollection();
        services.AddAndyConfiguration(configuration);
        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<AndyOptions>>().Value;

        // Assert
        // Check that specified values are set
        options.Model.Name.Should().Be("partial-model");
        options.Authentication.Method.Should().Be(AuthenticationMethod.ApiKey);
        options.UI.Theme.Should().Be("custom");
        
        // Check that defaults are used for unspecified values
        options.Model.Temperature.Should().Be(0.7); // Default
        options.UI.EnableSyntaxHighlighting.Should().BeTrue(); // Default
        options.Tools.DefaultTimeoutMs.Should().Be(30000); // Default
        options.Logging.WriteToFile.Should().BeTrue(); // Default
    }
}