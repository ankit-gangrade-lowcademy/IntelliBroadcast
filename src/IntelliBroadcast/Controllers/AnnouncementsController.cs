using IntelliBroadcast.Identity;
using IntelliBroadcast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliBroadcast.Controllers;

[Authorize]
public sealed class AnnouncementsController(IAnnouncementService announcements, ILogger<AnnouncementsController> logger) : Controller
{
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1) { ModelState.AddModelError(nameof(page), "Page must be at least 1."); page = 1; }
        if (!long.TryParse(User.FindFirst(AppClaimTypes.TenantId)?.Value, out var tenantId)) return Challenge();
        try { return View(await announcements.GetPageAsync(tenantId, page, 10, cancellationToken)); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to load announcements.");
            ViewData["LoadError"] = "Announcements could not be loaded. Please try again.";
            return View(new ViewModels.AnnouncementIndexViewModel { Page = 1, PageSize = 10 });
        }
    }
}
