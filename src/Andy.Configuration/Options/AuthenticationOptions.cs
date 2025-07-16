using System.ComponentModel.DataAnnotations;

namespace Andy.Configuration;

/// <summary>
/// Authentication configuration options.
/// </summary>
public class AuthenticationOptions
{
  /// <summary>
  /// Authentication method to use.
  /// </summary>
  [Required]
  public AuthenticationMethod Method { get; set; } = AuthenticationMethod.OAuth;

  /// <summary>
  /// Whether to cache authentication tokens.
  /// </summary>
  public bool CacheTokens { get; set; } = true;

  /// <summary>
  /// API key for direct authentication.
  /// </summary>
  public string? ApiKey { get; set; }

  /// <summary>
  /// OAuth client configuration.
  /// </summary>
  public OAuthOptions OAuth { get; set; } = new();

  /// <summary>
  /// Vertex AI configuration.
  /// </summary>
  public VertexAIOptions VertexAI { get; set; } = new();

  /// <summary>
  /// Gets or sets the default AI provider to use.
  /// </summary>
  public string DefaultProvider { get; set; } = "OpenAI";
}