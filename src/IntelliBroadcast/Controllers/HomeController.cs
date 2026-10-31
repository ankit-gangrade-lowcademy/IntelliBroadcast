using System.Diagnostics;
using IntelliBroadcast.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliBroadcast.Controllers;
public sealed class HomeController : Controller
{
    [AllowAnonymous]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
