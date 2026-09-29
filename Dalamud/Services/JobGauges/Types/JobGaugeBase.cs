namespace Dalamud.Services.JobGauges.Types;

/// <summary>
/// Base job gauge class.
/// </summary>
public abstract class JobGaugeBase : IJobGaugeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobGaugeBase"/> class.
    /// </summary>
    /// <param name="address">Address of the job gauge.</param>
    internal JobGaugeBase(nint address)
    {
        this.Address = address;
    }

    /// <inheritdoc/>
    public nint Address { get; }
}
