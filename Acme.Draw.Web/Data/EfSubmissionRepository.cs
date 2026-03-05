using Acme.Draw.Core;
using Microsoft.EntityFrameworkCore;

namespace Acme.Draw.Web.Data;

public sealed class EfSubmissionRepository : ISubmissionRepository
{
    private readonly DrawDbContext _db;

    public EfSubmissionRepository(DrawDbContext db) => _db = db;

    public Task<int> CountBySerialAsync(string serialNumber, CancellationToken ct = default)
    {
        var value = (serialNumber ?? string.Empty).Trim().ToUpperInvariant();
        return _db.Submissions.CountAsync(x => x.SerialNumber == value, ct);
    }

    public async Task AddAsync(StoredSubmission submission, CancellationToken ct = default)
    {
        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync(ct);
    }
}