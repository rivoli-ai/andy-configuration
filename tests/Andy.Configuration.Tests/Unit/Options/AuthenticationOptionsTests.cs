using FluentAssertions;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Unit.Options;

public class AuthenticationOptionsTests
{
    [Fact]
    public void AuthenticationOptions_DefaultValues_AreCorrect()
    {
        // Act
        var options = new AuthenticationOptions();

        // Assert
        options.Method.Should().Be(AuthenticationMethod.OAuth);
        options.CacheTokens.Should().BeTrue();
        options.ApiKey.Should().BeNull();
        options.OAuth.Should().NotBeNull();
        options.VertexAI.Should().NotBeNull();
        options.DefaultProvider.Should().Be("OpenAI");
    }

    [Fact]
    public void OAuthOptions_DefaultValues_AreCorrect()
    {
        // Act
        var options = new OAuthOptions();

        // Assert
        options.ClientId.Should().BeNull();
        options.ClientSecret.Should().BeNull();
        options.RedirectUri.Should().Be("http://localhost:8080/callback");
    }

    [Fact]
    public void VertexAIOptions_DefaultValues_AreCorrect()
    {
        // Act
        var options = new VertexAIOptions();

        // Assert
        options.ProjectId.Should().BeNull();
        options.Region.Should().Be("us-central1");
        options.ServiceAccountKeyPath.Should().BeNull();
    }

    [Theory]
    [InlineData(AuthenticationMethod.OAuth)]
    [InlineData(AuthenticationMethod.ApiKey)]
    [InlineData(AuthenticationMethod.VertexAI)]
    public void AuthenticationMethod_AllValuesAreValid(AuthenticationMethod method)
    {
        // Arrange
        var options = new AuthenticationOptions { Method = method };

        // Act & Assert
        options.Method.Should().Be(method);
    }
}