namespace Acme.Draw.Core;

public interface ISubmissionRepository
{
    /// <summary>Returns how many submissions already exist for a given serial number.</summary>
    Task<int> CountBySerialAsync(string serialNumber, CancellationToken ct = default);

    /// <summary>Persists a submission (implementation decides entity shape).</summary>
    Task AddAsync(StoredSubmission submission, CancellationToken ct = default);
}