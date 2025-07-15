using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Andy.Configuration;

/// <summary>
/// Validates configuration objects using data annotations.
/// </summary>
public class ConfigurationValidator<T> : IValidateOptions<T> where T : class
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, T options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("Configuration object cannot be null");
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(options);

        bool isValid = ValidationExtensions.TryValidateObjectRecursively(options, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage ?? "Unknown validation error").ToList();
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}

/// <summary>
/// Extension methods for validation.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates an object and all its properties recursively using data annotations.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <param name="validationResults">Collection to store validation results.</param>
    /// <param name="validateAllProperties">Whether to validate all properties.</param>
    /// <returns>True if validation passes, false otherwise.</returns>
    public static bool TryValidateObjectRecursively(object obj, ValidationContext validationContext, ICollection<ValidationResult> validationResults, bool validateAllProperties)
    {
        // Validate the current object
        bool result = Validator.TryValidateObject(obj, validationContext, validationResults, validateAllProperties);

        // Get all properties of the object
        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.CanRead)
            {
                var value = property.GetValue(obj);

                if (value == null)
                {
                    continue;
                }

                // Check if property is a complex type that should be validated recursively
                if (IsComplexType(property.PropertyType))
                {
                    var nestedContext = new ValidationContext(value);
                    if (!TryValidateObjectRecursively(value, nestedContext, validationResults, validateAllProperties))
                    {
                        result = false;
                    }
                }
                // Handle collections
                else if (IsEnumerableType(property.PropertyType))
                {
                    foreach (var item in (System.Collections.IEnumerable)value)
                    {
                        if (item != null && IsComplexType(item.GetType()))
                        {
                            var itemContext = new ValidationContext(item);
                            if (!TryValidateObjectRecursively(item, itemContext, validationResults, validateAllProperties))
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
        }

        return result;
    }

    private static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive &&
               !type.IsEnum &&
               type != typeof(string) &&
               type != typeof(DateTime) &&
               type != typeof(DateTimeOffset) &&
               type != typeof(TimeSpan) &&
               type != typeof(Guid) &&
               !type.IsValueType;
    }

    private static bool IsEnumerableType(Type type)
    {
        return type != typeof(string) &&
               typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
    }
}
