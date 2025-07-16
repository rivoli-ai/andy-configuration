using System.ComponentModel.DataAnnotations;

namespace Andy.Configuration;

/// <summary>
/// Root configuration options for Andy CLI.
/// </summary>
public class AndyOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Andy";

    /// <summary>
    /// Model configuration settings.
    /// </summary>
    [Required]
    public ModelOptions Model { get; set; } = new();

    /// <summary>
    /// Authentication configuration settings.
    /// </summary>
    [Required]
    public AuthenticationOptions Authentication { get; set; } = new();

    /// <summary>
    /// UI configuration settings.
    /// </summary>
    [Required]
    public UIOptions UI { get; set; } = new();

    /// <summary>
    /// Tool configuration settings.
    /// </summary>
    [Required]
    public ToolOptions Tools { get; set; } = new();

    /// <summary>
    /// Logging configuration settings.
    /// </summary>
    [Required]
    public LoggingOptions Logging { get; set; } = new();
}