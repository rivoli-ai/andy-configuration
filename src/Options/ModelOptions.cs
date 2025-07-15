using System.ComponentModel.DataAnnotations;

namespace Andy.Configuration;

/// <summary>
/// Model configuration options.
/// </summary>
public class ModelOptions
{
  /// <summary>
  /// The model name to use.
  /// </summary>
  [Required]
  public string Name { get; set; } = "gemini-2.0-flash-exp";

  /// <summary>
  /// Temperature for model responses (0.0 to 2.0).
  /// </summary>
  [Range(0.0, 2.0)]
  public double Temperature { get; set; } = 0.7;

  /// <summary>
  /// Maximum number of tokens in the response.
  /// </summary>
  [Range(1, 32768)]
  public int? MaxTokens { get; set; }

  /// <summary>
  /// Top-p sampling parameter.
  /// </summary>
  [Range(0.0, 1.0)]
  public double? TopP { get; set; }

  /// <summary>
  /// Top-k sampling parameter.
  /// </summary>
  [Range(1, 100)]
  public int? TopK { get; set; }
}