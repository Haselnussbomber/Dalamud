using System.Collections.Generic;

namespace Dalamud.Services.Marketboard.Structures;

/// <summary>
/// An interface that represents the market board history from the game.
/// </summary>
public interface IMarketBoardHistory
{
    /// <summary>
    /// Gets the item ID.
    /// </summary>
    uint ItemId { get; }

    /// <summary>
    /// Gets the list of individual item history listings.
    /// </summary>
    IReadOnlyList<IMarketBoardHistoryListing> HistoryListings { get; }
}
