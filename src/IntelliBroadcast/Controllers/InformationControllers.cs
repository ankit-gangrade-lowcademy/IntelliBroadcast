using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliBroadcast.Controllers;

[Authorize]
public sealed class RequirementsController : Controller { public IActionResult Index() => View(); }
[Authorize]
public sealed class ArchitectureController : Controller { public IActionResult Index() => View(); }
