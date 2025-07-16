using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Unit.Options;

public class ModelOptionsTests
{
    [Fact]
    public void ModelOptions_DefaultValues_AreCorrect()
    {
        // Act
        var options = new ModelOptions();

        // Assert
        options.Name.Should().Be("gemini-2.0-flash-exp");
        options.Temperature.Should().Be(0.7);
        options.MaxTokens.Should().BeNull();
        options.TopP.Should().BeNull();
        options.TopK.Should().BeNull();
    }

    [Theory]
    [InlineData(0.0, true)]
    [InlineData(0.5, true)]
    [InlineData(1.0, true)]
    [InlineData(2.0, true)]
    [InlineData(-0.1, false)]
    [InlineData(2.1, false)]
    public void Temperature_Validation_WorksCorrectly(double temperature, bool shouldBeValid)
    {
        // Arrange
        var options = new ModelOptions { Temperature = temperature };
        var validationContext = new ValidationContext(options) { MemberName = nameof(ModelOptions.Temperature) };
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateProperty(options.Temperature, validationContext, results);

        // Assert
        isValid.Should().Be(shouldBeValid);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(16384, true)]
    [InlineData(32768, true)]
    [InlineData(0, false)]
    [InlineData(32769, false)]
    public void MaxTokens_Validation_WorksCorrectly(int maxTokens, bool shouldBeValid)
    {
        // Arrange
        var options = new ModelOptions { MaxTokens = maxTokens };
        var validationContext = new ValidationContext(options) { MemberName = nameof(ModelOptions.MaxTokens) };
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateProperty(options.MaxTokens, validationContext, results);

        // Assert
        isValid.Should().Be(shouldBeValid);
    }

    [Theory]
    [InlineData(0.0, true)]
    [InlineData(0.5, true)]
    [InlineData(1.0, true)]
    [InlineData(-0.1, false)]
    [InlineData(1.1, false)]
    public void TopP_Validation_WorksCorrectly(double topP, bool shouldBeValid)
    {
        // Arrange
        var options = new ModelOptions { TopP = topP };
        var validationContext = new ValidationContext(options) { MemberName = nameof(ModelOptions.TopP) };
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateProperty(options.TopP, validationContext, results);

        // Assert
        isValid.Should().Be(shouldBeValid);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(50, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(101, false)]
    public void TopK_Validation_WorksCorrectly(int topK, bool shouldBeValid)
    {
        // Arrange
        var options = new ModelOptions { TopK = topK };
        var validationContext = new ValidationContext(options) { MemberName = nameof(ModelOptions.TopK) };
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateProperty(options.TopK, validationContext, results);

        // Assert
        isValid.Should().Be(shouldBeValid);
    }
}