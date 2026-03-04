namespace Acme.Draw.Core;

/// <summary>
/// Core-level persisted model. Keep it simple; infra layer can map it to EF entity.
/// </summary>
public sealed record StoredSubmission(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateOnly DateOfBirth,
    string SerialNumber,
    DateTimeOffset CreatedAtUtc
);