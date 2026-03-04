namespace Acme.Draw.Core;

public enum SubmissionErrorCode
{
    None = 0,
    InvalidSerialNumber,
    UnderAge,
    SerialNumberAlreadyUsedTwice,
    InvalidEmail,
    InvalidName
}