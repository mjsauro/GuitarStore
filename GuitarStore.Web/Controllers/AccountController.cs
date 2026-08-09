using GuitarStore.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

/// <summary>
/// Sign-in and sign-out. Cognito's hosted UI owns the actual login, registration,
/// email confirmation, and password reset screens — in the MVC 5 version those were seven
/// hand-written views and controller actions, several of which had no antiforgery token.
/// </summary>
public class AccountController : Controller
{
    private readonly CognitoOptions _cognito;

    public AccountController(CognitoOptions cognito) => _cognito = cognito;

    /// <summary>Hands off to the hosted UI; Cognito redirects back to /signin-oidc.</summary>
    [HttpGet]
    public IActionResult SignIn(string? returnUrl = null)
    {
        if (!_cognito.IsConfigured)
        {
            return RedirectToAction("Index", "DevAuth");
        }

        var redirect = Url.IsLocalUrl(returnUrl) ? returnUrl! : Url.Action("Index", "Home")!;
        return Challenge(
            new AuthenticationProperties { RedirectUri = redirect },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Clears the local session, then sends the browser to Cognito's logout endpoint so the
    /// hosted-UI session goes too — otherwise signing back in would silently reuse it.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SignOutUser()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!_cognito.IsConfigured)
        {
            return RedirectToAction("Index", "Home");
        }

        var home = $"{PublicOrigin()}/";
        return Redirect(_cognito.BuildLogoutUrl(home));
    }

    [HttpGet]
    public IActionResult Denied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        ViewData["Title"] = "Access Denied";
        return View();
    }

    [Authorize]
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Your Account";
        return View();
    }

    private string PublicOrigin()
    {
        var configured = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["App:PublicOrigin"];

        return string.IsNullOrEmpty(configured)
            ? $"{Request.Scheme}://{Request.Host}"
            : configured.TrimEnd('/');
    }
}
