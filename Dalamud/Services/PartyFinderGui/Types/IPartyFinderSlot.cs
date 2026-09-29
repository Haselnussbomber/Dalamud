using System.Collections.Generic;

namespace Dalamud.Services.PartyFinderGui.Types;

/// <summary>
/// A player slot in a Party Finder listing.
/// </summary>
public interface IPartyFinderSlot
{
    /// <summary>
    /// Gets a list of jobs that this slot is accepting.
    /// </summary>
    IReadOnlyCollection<JobFlags> Accepting { get; }

    /// <summary>
    /// Tests if this slot is accepting a job.
    /// </summary>
    /// <param name="flag">Job to test.</param>
    bool this[JobFlags flag] { get; }
}
