namespace GuitarStore.Web.Services;

/// <summary>
/// Cognito wiring. When <see cref="IsConfigured"/> is false the app falls back to the
/// Development sign-in stub, so a fresh clone runs without any AWS setup.
/// </summary>
public class CognitoOptions
{
    public const string SectionName = "Cognito";

    public string UserPoolId { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public string Region { get; set; } = "us-east-2";

    /// <summary>The hosted UI domain prefix, e.g. "guitarstore-example".</summary>
    public string DomainPrefix { get; set; } = "";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(UserPoolId) &&
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(DomainPrefix);

    /// <summary>OIDC issuer — where the discovery document lives.</summary>
    public string Authority => $"https://cognito-idp.{Region}.amazonaws.com/{UserPoolId}";

    /// <summary>Hosted UI base, which serves the sign-in, sign-up, and logout pages.</summary>
    public string HostedUiBaseUrl => $"https://{DomainPrefix}.auth.{Region}.amazoncognito.com";

    /// <summary>
    /// Cognito's logout endpoint isn't the standard OIDC end_session_endpoint, so signing
    /// out has to be built by hand rather than left to the OIDC handler.
    /// </summary>
    public string BuildLogoutUrl(string returnUrl) =>
        $"{HostedUiBaseUrl}/logout?client_id={Uri.EscapeDataString(ClientId)}&logout_uri={Uri.EscapeDataString(returnUrl)}";
}
