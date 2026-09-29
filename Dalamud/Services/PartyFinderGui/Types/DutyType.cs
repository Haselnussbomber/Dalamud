namespace Dalamud.Services.PartyFinderGui.Types;

/// <summary>
/// Duty type flags for <see cref="IPartyFinderListing"/>.
/// </summary>
public enum DutyType
{
    /// <summary>
    /// No duty type.
    /// </summary>
    Other = 0,

    /// <summary>
    /// The roulette duty type.
    /// </summary>
    Roulette = 1 << 0,

    /// <summary>
    /// The normal duty type.
    /// </summary>
    Normal = 1 << 1,
}
