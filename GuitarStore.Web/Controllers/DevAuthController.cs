using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

/// <summary>
/// A stand-in for Cognito so the app can be run and tested locally without AWS.
///
/// Every action refuses to do anything outside the Development environment, and
/// Program.cs only maps this controller's routes in Development — belt and braces, because
/// an endpoint that hands out admin sessions must never be reachable in production.
/// </summary>
public class DevAuthController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public DevAuthController(IWebHostEnvironment environment) => _environment = environment;

    [HttpGet]
    public IActionResult Index()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        ViewData["Title"] = "Developer Sign In";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignInAs(string email, bool isAdmin, CancellationToken ct)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, $"dev|{email}"),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email)
        };

        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, Roles.Admin));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }
}
