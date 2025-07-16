namespace Andy.Configuration;

/// <summary>
/// Vertex AI configuration options.
/// </summary>
public class VertexAIOptions
{
  /// <summary>
  /// Google Cloud project ID.
  /// </summary>
  public string? ProjectId { get; set; }

  /// <summary>
  /// Google Cloud region.
  /// </summary>
  public string Region { get; set; } = "us-central1";

  /// <summary>
  /// Service account key file path.
  /// </summary>
  public string? ServiceAccountKeyPath { get; set; }
}