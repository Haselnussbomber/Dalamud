namespace Dalamud.Services.JobGauges.Types;

/// <summary>
/// Interface for all JobGauges.
/// </summary>
public interface IJobGaugeBase
{
    /// <summary>
    /// Gets the address of this job gauge in memory.
    /// </summary>
    nint Address { get; }
}
