using System.Security.Claims;
using IntelliBroadcast.Web.Data;
using IntelliBroadcast.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace IntelliBroadcast.Web.Controllers;
[Authorize]
public sealed class AnnouncementsController(IntelliBroadcastDbContext db, ILogger<AnnouncementsController> logger) : Controller
{
    private const int PageSize = 10;
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1) return BadRequest();
        if (!long.TryParse(User.FindFirstValue("TenantID"), out var tenantId)) return Challenge();
        try
        {
            var query = db.Broadcasts.AsNoTracking().Where(x => x.TenantID == tenantId || x.TenantID == 0);
            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            if (page > totalPages) page = totalPages;
            var items = await query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ID)
                .Skip((page - 1) * PageSize).Take(PageSize)
                .Select(x => new AnnouncementRowViewModel(x.ID, x.Message, x.Description, x.CreatedDate, x.TenantID == 0))
                .ToListAsync(cancellationToken);
            return View(new AnnouncementsViewModel { Items = items, Page = page, PageSize = PageSize, TotalCount = totalCount });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve announcements.");
            ViewData["LoadError"] = "Announcements could not be loaded. Please try again.";
            return View(new AnnouncementsViewModel { Items = [], Page = 1, PageSize = PageSize, TotalCount = 0 });
        }
    }
}
