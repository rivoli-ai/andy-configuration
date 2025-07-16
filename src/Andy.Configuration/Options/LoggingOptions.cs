using System.ComponentModel.DataAnnotations;

namespace Andy.Configuration;

/// <summary>
/// Logging configuration options.
/// </summary>
public class LoggingOptions
{
  /// <summary>
  /// Minimum log level.
  /// </summary>
  [Required]
  public string MinimumLevel { get; set; } = "Information";

  /// <summary>
  /// Whether to write logs to file.
  /// </summary>
  public bool WriteToFile { get; set; } = true;

  /// <summary>
  /// Log file path.
  /// </summary>
  public string FilePath { get; set; } = "logs/andy-.log";

  /// <summary>
  /// Whether to write logs to console.
  /// </summary>
  public bool WriteToConsole { get; set; } = true;

  /// <summary>
  /// Whether to show user-friendly API connection messages instead of technical logs.
  /// </summary>
  public bool ShowUserFriendlyApiMessages { get; set; } = true;
}