using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Unit.Validation;

public class ConfigurationValidatorTests
{
    private readonly ConfigurationValidator<TestOptions> _validator;

    public ConfigurationValidatorTests()
    {
        _validator = new ConfigurationValidator<TestOptions>();
    }

    [Fact]
    public void Validate_WithNullOptions_ReturnsFailure()
    {
        // Act
        var result = _validator.Validate(null, null!);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Be("Configuration object cannot be null");
    }

    [Fact]
    public void Validate_WithValidOptions_ReturnsSuccess()
    {
        // Arrange
        var options = new TestOptions
        {
            RequiredString = "value",
            RangeValue = 50,
            NestedOptions = new NestedTestOptions
            {
                RequiredNestedString = "nested value"
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.FailureMessage.Should().BeNull();
    }

    [Fact]
    public void Validate_WithMissingRequiredField_ReturnsFailure()
    {
        // Arrange
        var options = new TestOptions
        {
            RequiredString = null!,
            RangeValue = 50,
            NestedOptions = new NestedTestOptions
            {
                RequiredNestedString = "nested value"
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failures.Should().Contain(f => f.Contains("RequiredString"));
    }

    [Fact]
    public void Validate_WithRangeViolation_ReturnsFailure()
    {
        // Arrange
        var options = new TestOptions
        {
            RequiredString = "value",
            RangeValue = 150, // Out of range
            NestedOptions = new NestedTestOptions
            {
                RequiredNestedString = "nested value"
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failures.Should().Contain(f => f.Contains("RangeValue"));
    }

    [Fact]
    public void Validate_WithNestedValidationError_ReturnsFailure()
    {
        // Arrange
        var options = new TestOptions
        {
            RequiredString = "value",
            RangeValue = 50,
            NestedOptions = new NestedTestOptions
            {
                RequiredNestedString = null! // Missing required nested field
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failures.Should().Contain(f => f.Contains("RequiredNestedString"));
    }

    [Fact]
    public void Validate_WithCollectionProperty_ValidatesCollectionExists()
    {
        // Arrange
        var options = new TestOptionsWithCollection
        {
            Items = null! // Collection itself is null which violates Required attribute
        };

        var validator = new ConfigurationValidator<TestOptionsWithCollection>();

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failures.Should().Contain(f => f.Contains("Items"));
    }

    // Test classes
    private class TestOptions
    {
        [Required]
        public string RequiredString { get; set; } = default!;

        [Range(1, 100)]
        public int RangeValue { get; set; }

        [Required]
        public NestedTestOptions NestedOptions { get; set; } = default!;
    }

    private class NestedTestOptions
    {
        [Required]
        public string RequiredNestedString { get; set; } = default!;
    }

    private class TestOptionsWithCollection
    {
        [Required]
        public List<string> Items { get; set; } = new();
    }
}