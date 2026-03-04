namespace Acme.Draw.Core;

public sealed class DrawEntryService
{
    private readonly ISerialNumberRepository _serials;
    private readonly ISubmissionRepository _submissions;

    public DrawEntryService(ISerialNumberRepository serials, ISubmissionRepository submissions)
    {
        _serials = serials;
        _submissions = submissions;
    }

    public async Task<SubmissionResult> SubmitAsync(SubmissionRequest req, DateOnly today, CancellationToken ct = default)
    {
        // Basic sanitization (trim only; don't get fancy in MVP)
        var first = (req.FirstName ?? string.Empty).Trim();
        var last = (req.LastName ?? string.Empty).Trim();
        var email = (req.Email ?? string.Empty).Trim();
        var serial = (req.SerialNumber ?? string.Empty).Trim().ToUpperInvariant();

        if (first.Length == 0 || last.Length == 0)
            return SubmissionResult.Fail(SubmissionErrorCode.InvalidName, "First name and last name are required.");

        if (!IsValidEmail(email))
            return SubmissionResult.Fail(SubmissionErrorCode.InvalidEmail, "Email is invalid.");

        // Age check: must be >= 18
        if (!IsAtLeast18(req.DateOfBirth, today))
            return SubmissionResult.Fail(SubmissionErrorCode.UnderAge, "You must be at least 18 years old.");

        // Serial must exist
        if (!await _serials.ExistsAsync(serial, ct))
            return SubmissionResult.Fail(SubmissionErrorCode.InvalidSerialNumber, "Serial number is invalid.");

        // Max 2 submissions per serial number
        var count = await _submissions.CountBySerialAsync(serial, ct);
        if (count >= 2)
            return SubmissionResult.Fail(SubmissionErrorCode.SerialNumberAlreadyUsedTwice, "This serial number has already been used twice.");

        // Persist
        var stored = new StoredSubmission(
            Id: Guid.NewGuid(),
            FirstName: first,
            LastName: last,
            Email: email,
            DateOfBirth: req.DateOfBirth,
            SerialNumber: serial,
            CreatedAtUtc: DateTimeOffset.UtcNow
        );

        await _submissions.AddAsync(stored, ct);
        return SubmissionResult.Ok();
    }

    private static bool IsAtLeast18(DateOnly dob, DateOnly today)
    {
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age >= 18;
    }

    // Simple validation for the exercise (not full RFC compliance)
    private static bool IsValidEmail(string email)
    {
        if (email.Length < 5) return false;
        // simple: contains one @ and at least one dot after @
        var at = email.IndexOf('@');
        if (at <= 0 || at != email.LastIndexOf('@')) return false;
        var dot = email.LastIndexOf('.');
        return dot > at + 1 && dot < email.Length - 1;
    }
}