using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IntelliBroadcast.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace IntelliBroadcast.Web.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index() => User.Identity?.IsAuthenticated == true
        ? RedirectToAction("Index", "Announcements") : RedirectToAction("Login", "Account");

    [AllowAnonymous]
    public IActionResult Error(int? statusCode = null)
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = statusCode });
    }
}
