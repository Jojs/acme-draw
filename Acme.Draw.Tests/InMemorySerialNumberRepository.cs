using Acme.Draw.Core;

namespace Acme.Draw.Tests;

internal sealed class InMemorySerialNumberRepository : ISerialNumberRepository
{
    private readonly HashSet<string> _serials;

    public InMemorySerialNumberRepository(IEnumerable<string> serials)
    {
        _serials = new HashSet<string>(serials.Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
    }

    public Task<bool> ExistsAsync(string serialNumber, CancellationToken ct = default)
        => Task.FromResult(_serials.Contains(serialNumber.Trim()));
}