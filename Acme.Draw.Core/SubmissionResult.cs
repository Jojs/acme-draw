namespace Acme.Draw.Core;

public sealed record SubmissionResult(bool IsSuccess, SubmissionErrorCode ErrorCode, string? ErrorMessage = null)
{
    public static SubmissionResult Ok() =>
        new(true, SubmissionErrorCode.None);

    public static SubmissionResult Fail(SubmissionErrorCode code, string message) =>
        new(false, code, message);
}