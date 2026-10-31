using System.Security.Claims;
using IntelliBroadcast.Identity;
using IntelliBroadcast.Services;
using IntelliBroadcast.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliBroadcast.Controllers;

public sealed class AccountController(IUserAuthenticationService authenticationService, ILogger<AccountController> logger) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Announcements");
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await authenticationService.ValidateAsync(model.Email, model.Password, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("An authentication attempt failed.");
            ModelState.AddModelError(string.Empty, "The email or password is incorrect.");
            return View(model);
        }
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, string.Join(' ', new[] { user.Name, user.Surname }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim() is { Length: > 0 } fullName ? fullName : user.Email!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(AppClaimTypes.UserId, user.Id.ToString()),
            new Claim(AppClaimTypes.TenantId, user.TenantId.ToString())
        };
        HttpContext.Session.SetString(SessionKeys.UserId, user.Id.ToString());
        HttpContext.Session.SetString(SessionKeys.TenantId, user.TenantId.ToString());
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl) : RedirectToAction("Index", "Announcements");
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}
