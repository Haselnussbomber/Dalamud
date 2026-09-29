using System.Collections.Generic;
using System.Linq;

namespace Dalamud.Services.PartyFinderGui.Types;

/// <inheritdoc/>
internal class PartyFinderSlot : IPartyFinderSlot
{
    private readonly ulong accepting;
    private JobFlags[] listAccepting;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyFinderSlot"/> class.
    /// </summary>
    /// <param name="accepting">The flag value of accepted jobs.</param>
    internal PartyFinderSlot(ulong accepting)
    {
        this.accepting = accepting;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<JobFlags> Accepting => this.listAccepting ??= Enum.GetValues<JobFlags>().Where(flag => this[flag]).ToArray();

    /// <inheritdoc/>
    public bool this[JobFlags flag] => (this.accepting & (uint)flag) > 0;
}
