namespace Andy.Configuration;

/// <summary>
/// OAuth configuration options.
/// </summary>
public class OAuthOptions
{
  /// <summary>
  /// OAuth client ID.
  /// </summary>
  public string? ClientId { get; set; }

  /// <summary>
  /// OAuth client secret.
  /// </summary>
  public string? ClientSecret { get; set; }

  /// <summary>
  /// OAuth redirect URI.
  /// </summary>
  public string RedirectUri { get; set; } = "http://localhost:8080/callback";
}