using Microsoft.EntityFrameworkCore;

namespace Acme.Draw.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(DrawDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.SerialNumbers.AnyAsync(ct))
            return;

        var serials = Enumerable.Range(1, 100)
            .Select(i => new SerialNumber { Value = $"SN{i:0000}" })
            .ToList();

        db.SerialNumbers.AddRange(serials);
        await db.SaveChangesAsync(ct);
    }
}