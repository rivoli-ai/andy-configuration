using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;
using Andy.Configuration;

namespace Andy.Configuration.Tests.Unit.Validation;

public class ValidationExtensionsTests
{
    [Fact]
    public void TryValidateObjectRecursively_WithValidObject_ReturnsTrue()
    {
        // Arrange
        var obj = new ComplexObject
        {
            Name = "Test",
            Value = 50,
            NestedObject = new NestedObject
            {
                Description = "Nested",
                IsActive = true
            }
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        // Act
        var isValid = ValidationExtensions.TryValidateObjectRecursively(obj, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void TryValidateObjectRecursively_WithInvalidRootProperty_ReturnsFalse()
    {
        // Arrange
        var obj = new ComplexObject
        {
            Name = null!, // Required field
            Value = 50,
            NestedObject = new NestedObject
            {
                Description = "Nested",
                IsActive = true
            }
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        // Act
        var isValid = ValidationExtensions.TryValidateObjectRecursively(obj, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.ErrorMessage!.Contains("Name"));
    }

    [Fact]
    public void TryValidateObjectRecursively_WithInvalidNestedProperty_ReturnsFalse()
    {
        // Arrange
        var obj = new ComplexObject
        {
            Name = "Test",
            Value = 50,
            NestedObject = new NestedObject
            {
                Description = null!, // Required field
                IsActive = true
            }
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        // Act
        var isValid = ValidationExtensions.TryValidateObjectRecursively(obj, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.ErrorMessage!.Contains("Description"));
    }

    [Fact]
    public void TryValidateObjectRecursively_WithNullNestedObject_ReturnsTrue()
    {
        // Arrange
        var obj = new ComplexObject
        {
            Name = "Test",
            Value = 50,
            NestedObject = null! // Not marked as required
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        // Act
        var isValid = ValidationExtensions.TryValidateObjectRecursively(obj, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void TryValidateObjectRecursively_WithNullCollection_ValidatesRequired()
    {
        // Arrange
        var obj = new ObjectWithCollection
        {
            Items = null! // Null collection violates Required attribute
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        // Act
        var isValid = ValidationExtensions.TryValidateObjectRecursively(obj, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.ErrorMessage!.Contains("Items"));
    }

    [Fact]
    public void TryValidateObjectRecursively_WithPrimitiveProperties_SkipsRecursion()
    {
        // Arrange
        var obj = new PrimitiveObject
        {
            StringValue = "test",
            IntValue = 42,
            BoolValue = true,
            DateValue = DateTime.Now,
            GuidValue = Guid.NewGuid()
        };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        // Act
        var isValid = ValidationExtensions.TryValidateObjectRecursively(obj, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    // Test classes
    private class ComplexObject
    {
        [Required]
        public string Name { get; set; } = default!;

        [Range(0, 100)]
        public int Value { get; set; }

        public NestedObject NestedObject { get; set; } = default!;
    }

    private class NestedObject
    {
        [Required]
        public string Description { get; set; } = default!;

        public bool IsActive { get; set; }
    }

    private class ObjectWithCollection
    {
        [Required]
        public List<string> Items { get; set; } = new();
    }

    private class PrimitiveObject
    {
        [Required]
        public string StringValue { get; set; } = default!;

        public int IntValue { get; set; }
        public bool BoolValue { get; set; }
        public DateTime DateValue { get; set; }
        public Guid GuidValue { get; set; }
    }
}