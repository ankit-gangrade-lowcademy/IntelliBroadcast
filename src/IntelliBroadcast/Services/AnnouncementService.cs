using IntelliBroadcast.Data;
using IntelliBroadcast.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace IntelliBroadcast.Services;

public interface IAnnouncementService
{
    Task<AnnouncementIndexViewModel> GetPageAsync(long tenantId, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed class AnnouncementService(ApplicationDbContext db) : IAnnouncementService
{
    public async Task<AnnouncementIndexViewModel> GetPageAsync(long tenantId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.Broadcasts.AsNoTracking().Where(x => x.TenantId == tenantId || x.TenantId == 0);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new AnnouncementItemViewModel(x.Id, x.Message, x.Description, x.CreatedDate, x.TenantId == 0))
            .ToListAsync(cancellationToken);
        return new AnnouncementIndexViewModel { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }
}
