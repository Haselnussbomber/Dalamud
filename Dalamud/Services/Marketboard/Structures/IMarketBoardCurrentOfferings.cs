using System.Collections.Generic;

namespace Dalamud.Services.Marketboard.Structures;

/// <summary>
/// An interface that represents the current market board offerings.
/// </summary>
public interface IMarketBoardCurrentOfferings
{
    /// <summary>
    /// Gets the list of individual item listings.
    /// </summary>
    IReadOnlyList<IMarketBoardItemListing> ItemListings { get; }

    /// <summary>
    /// Gets the request ID.
    /// </summary>
    int RequestId { get; }
}
