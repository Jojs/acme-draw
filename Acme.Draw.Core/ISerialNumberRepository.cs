namespace Acme.Draw.Core;

public interface ISerialNumberRepository
{
    /// <summary>Returns true if the serial number exists in the valid serial list.</summary>
    Task<bool> ExistsAsync(string serialNumber, CancellationToken ct = default);
}