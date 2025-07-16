using System.ComponentModel.DataAnnotations;

namespace Andy.Configuration;

/// <summary>
/// Tool configuration options.
/// </summary>
public class ToolOptions
{
  /// <summary>
  /// Whether to require confirmation for dangerous operations.
  /// </summary>
  public bool RequireConfirmation { get; set; } = true;

  /// <summary>
  /// Default timeout for tool execution in milliseconds.
  /// </summary>
  [Range(1000, 300000)]
  public int DefaultTimeoutMs { get; set; } = 30000;

  /// <summary>
  /// Maximum file size for read operations in bytes.
  /// </summary>
  [Range(1024, 100 * 1024 * 1024)]
  public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
}