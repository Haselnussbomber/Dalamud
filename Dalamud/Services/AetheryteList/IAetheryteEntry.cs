using Lumina.Excel;

namespace Dalamud.Services.AetheryteList;

/// <summary>
/// Interface representing an aetheryte entry available to the game.
/// </summary>
public interface IAetheryteEntry
{
    /// <summary>
    /// Gets the Aetheryte ID.
    /// </summary>
    uint AetheryteId { get; }

    /// <summary>
    /// Gets the Territory ID.
    /// </summary>
    uint TerritoryId { get; }

    /// <summary>
    /// Gets the SubIndex used when there can be multiple Aetherytes with the same ID (Private/Shared Estates etc.).
    /// </summary>
    byte SubIndex { get; }

    /// <summary>
    /// Gets the Ward. Zero if not a Shared Estate.
    /// </summary>
    byte Ward { get; }

    /// <summary>
    /// Gets the Plot. Zero if not a Shared Estate.
    /// </summary>
    byte Plot { get; }

    /// <summary>
    /// Gets the Cost in Gil to Teleport to this location.
    /// </summary>
    uint GilCost { get; }

    /// <summary>
    /// Gets a value indicating whether the LocalPlayer has set this Aetheryte as Favorite or not.
    /// </summary>
    bool IsFavourite { get; }

    /// <summary>
    /// Gets a value indicating whether this Aetheryte is a Shared Estate or not.
    /// </summary>
    bool IsSharedHouse { get; }

    /// <summary>
    /// Gets a value indicating whether this Aetheryte is an Apartment or not.
    /// </summary>
    bool IsApartment { get; }

    /// <summary>
    /// Gets the Aetheryte data related to this aetheryte.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.Aetheryte> AetheryteData { get; }
}
