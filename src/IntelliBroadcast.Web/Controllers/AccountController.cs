using System.Security.Claims;
using IntelliBroadcast.Web.Services;
using IntelliBroadcast.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace IntelliBroadcast.Web.Controllers;
public sealed class AccountController(ILoginService loginService) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Announcements");
        return View(new LoginViewModel { ReturnUrl = IsLocalUrl(returnUrl) ? returnUrl : null });
    }
    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ModelState.Clear();
            ModelState.AddModelError(string.Empty, LoginResult.ErrorMessage);
            return View(new LoginViewModel { Email = model.Email, ReturnUrl = IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl : null });
        }
        var result = await loginService.AuthenticateAsync(model.Email, model.Password, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, LoginResult.ErrorMessage);
            model.Password = string.Empty;
            return View(model);
        }
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()), new Claim(ClaimTypes.Email, result.Email),
            new Claim(ClaimTypes.Name, result.Email), new Claim("TenantID", result.TenantId.ToString()),
            new Claim(ClaimTypes.Role, result.RoleId.ToString()) };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties { IsPersistent = false, AllowRefresh = true });
        HttpContext.Session.SetString("UserID", result.UserId.ToString());
        HttpContext.Session.SetString("TenantID", result.TenantId.ToString());
        HttpContext.Session.SetInt32("ProductUserRoleID", result.RoleId);
        return IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl!) : RedirectToAction("Index", "Announcements");
    }
    [Authorize, HttpPost]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
    private bool IsLocalUrl(string? url) => !string.IsNullOrWhiteSpace(url) && Url.IsLocalUrl(url);
}
