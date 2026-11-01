using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace IntelliBroadcast.Web.Controllers;
[Authorize] public sealed class RequirementsController : Controller { public IActionResult Index() => View(); }
