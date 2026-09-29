namespace Dalamud.Services.Marketboard.Structures;

/// <summary>
/// An interface that represents the market board history of a single item from <see cref="IMarketBoardHistory"/>.
/// </summary>
public interface IMarketBoardHistoryListing
{
    /// <summary>
    /// Gets the buyer's name.
    /// </summary>
    string BuyerName { get; }

    /// <summary>
    /// Gets a value indicating whether the item is HQ.
    /// </summary>
    bool IsHq { get; }

    /// <summary>
    /// Gets a value indicating whether the item is on a mannequin.
    /// </summary>
    bool OnMannequin { get; }

    /// <summary>
    /// Gets the time of purchase.
    /// </summary>
    DateTime PurchaseTime { get; }

    /// <summary>
    /// Gets the quantity.
    /// </summary>
    uint Quantity { get; }

    /// <summary>
    /// Gets the sale price.
    /// </summary>
    uint SalePrice { get; }
}
