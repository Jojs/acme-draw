using Acme.Draw.Core;
using Microsoft.EntityFrameworkCore;

namespace Acme.Draw.Web.Data;

public sealed class EfSerialNumberRepository : ISerialNumberRepository
{
    private readonly DrawDbContext _db;

    public EfSerialNumberRepository(DrawDbContext db) => _db = db;

    public Task<bool> ExistsAsync(string serialNumber, CancellationToken ct = default)
    {
        var value = (serialNumber ?? string.Empty).Trim().ToUpperInvariant();
        return _db.SerialNumbers.AnyAsync(x => x.Value == value, ct);
    }
}