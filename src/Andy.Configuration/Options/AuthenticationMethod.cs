namespace Andy.Configuration;

/// <summary>
/// Authentication methods.
/// </summary>
public enum AuthenticationMethod
{
  /// <summary>
  /// OAuth authentication.
  /// </summary>
  OAuth,

  /// <summary>
  /// API key authentication.
  /// </summary>
  ApiKey,

  /// <summary>
  /// Vertex AI authentication.
  /// </summary>
  VertexAI
}