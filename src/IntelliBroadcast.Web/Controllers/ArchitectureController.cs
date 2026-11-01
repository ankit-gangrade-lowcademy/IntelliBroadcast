using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace IntelliBroadcast.Web.Controllers;
[Authorize] public sealed class ArchitectureController : Controller { public IActionResult Index() => View(); }
