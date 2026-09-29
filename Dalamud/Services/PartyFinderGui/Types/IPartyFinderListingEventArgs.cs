namespace Dalamud.Services.PartyFinderGui.Types;

/// <summary>
/// A interface representing  additional arguments passed by the game.
/// </summary>
public interface IPartyFinderListingEventArgs
{
    /// <summary>
    /// Gets the batch number.
    /// </summary>
    int BatchNumber { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the listing is visible.
    /// </summary>
    bool Visible { get; set; }
}
