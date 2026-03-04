using Acme.Draw.Core;

namespace Acme.Draw.Tests;

internal sealed class InMemorySubmissionRepository : ISubmissionRepository
{
    private readonly List<StoredSubmission> _items = [];
    public IReadOnlyList<StoredSubmission> Items => _items;

    public Task<int> CountBySerialAsync(string serialNumber, CancellationToken ct = default)
    {
        var count = _items.Count(x => string.Equals(x.SerialNumber, serialNumber.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(count);
    }

    public Task AddAsync(StoredSubmission submission, CancellationToken ct = default)
    {
        _items.Add(submission);
        return Task.CompletedTask;
    }
}