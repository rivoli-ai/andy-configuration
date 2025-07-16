using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Unit.Options;

public class AndyOptionsTests
{
    [Fact]
    public void AndyOptions_HasCorrectSectionName()
    {
        // Assert
        AndyOptions.SectionName.Should().Be("Andy");
    }

    [Fact]
    public void AndyOptions_DefaultConstructor_InitializesAllProperties()
    {
        // Act
        var options = new AndyOptions();

        // Assert
        options.Model.Should().NotBeNull();
        options.Authentication.Should().NotBeNull();
        options.UI.Should().NotBeNull();
        options.Tools.Should().NotBeNull();
        options.Logging.Should().NotBeNull();
    }

    [Fact]
    public void AndyOptions_AllPropertiesHaveRequiredAttribute()
    {
        // Arrange
        var type = typeof(AndyOptions);
        var propertiesToCheck = new[] { "Model", "Authentication", "UI", "Tools", "Logging" };

        // Act & Assert
        foreach (var propertyName in propertiesToCheck)
        {
            var property = type.GetProperty(propertyName);
            property.Should().NotBeNull();
            property!.GetCustomAttributes(typeof(RequiredAttribute), false)
                .Should().HaveCount(1, $"Property {propertyName} should have Required attribute");
        }
    }

    [Fact]
    public void AndyOptions_Validation_FailsWhenRequiredPropertiesAreNull()
    {
        // Arrange
        var options = new AndyOptions
        {
            Model = null!,
            Authentication = new AuthenticationOptions(),
            UI = new UIOptions(),
            Tools = new ToolOptions(),
            Logging = new LoggingOptions()
        };
        var validationContext = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(options, validationContext, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Model"));
    }
}