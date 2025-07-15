using System.ComponentModel.DataAnnotations;

namespace Andy.Configuration;

/// <summary>
/// UI configuration options.
/// </summary>
public class UIOptions
{
  /// <summary>
  /// UI theme name.
  /// </summary>
  [Required]
  public string Theme { get; set; } = "default";

  /// <summary>
  /// Whether to enable syntax highlighting.
  /// </summary>
  public bool EnableSyntaxHighlighting { get; set; } = true;

  /// <summary>
  /// Whether to enable auto-scrolling.
  /// </summary>
  public bool EnableAutoScroll { get; set; } = true;

  /// <summary>
  /// Maximum number of messages to display.
  /// </summary>
  [Range(10, 1000)]
  public int MaxDisplayMessages { get; set; } = 100;
}