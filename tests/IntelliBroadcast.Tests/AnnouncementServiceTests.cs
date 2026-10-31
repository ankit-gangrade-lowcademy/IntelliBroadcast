using IntelliBroadcast.Data;
using IntelliBroadcast.Models.Entities;
using IntelliBroadcast.Services;
using Microsoft.EntityFrameworkCore;

namespace IntelliBroadcast.Tests;

public sealed class AnnouncementServiceTests
{
    [Fact]
    public async Task ReturnsTenantAndGlobalRecords_InRequiredOrder_WithTenRows()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new ApplicationDbContext(options);
        var origin = new DateTime(2026, 1, 1);
        for (var i = 1; i <= 14; i++) db.Broadcasts.Add(new NorviaBroadcast { Id = i, TenantId = i == 14 ? 99 : i % 2 == 0 ? 0 : 7, Message = $"Message {i}", CreatedDate = origin.AddDays(i / 2) });
        await db.SaveChangesAsync();
        var result = await new AnnouncementService(db).GetPageAsync(7, 1, 10);
        Assert.Equal(13, result.TotalCount);
        Assert.Equal(10, result.Items.Count);
        Assert.DoesNotContain(result.Items, x => x.Id == 14);
        Assert.Equal(result.Items.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id).Select(x => x.Id), result.Items.Select(x => x.Id));
    }
}
