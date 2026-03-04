namespace Acme.Draw.Core;

public sealed record SubmissionRequest(
    string FirstName,
    string LastName,
    string Email,
    DateOnly DateOfBirth,
    string SerialNumber
);