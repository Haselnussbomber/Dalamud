using Dalamud.Services.DataManager;

using FFXIVClientStructs.FFXIV.Client.Game.UI;

using Lumina.Excel;

namespace Dalamud.Services.AetheryteList;

/// <summary>
/// This struct represents an aetheryte entry available to the game.
/// </summary>
/// <param name="data">Data read from the Aetheryte List.</param>
internal readonly struct AetheryteEntry(TeleportInfo data) : IAetheryteEntry
{
    /// <inheritdoc />
    public uint AetheryteId => data.AetheryteId;

    /// <inheritdoc />
    public uint TerritoryId => data.TerritoryId;

    /// <inheritdoc />
    public byte SubIndex => data.SubIndex;

    /// <inheritdoc />
    public byte Ward => data.Ward;

    /// <inheritdoc />
    public byte Plot => data.Plot;

    /// <inheritdoc />
    public uint GilCost => data.GilCost;

    /// <inheritdoc />
    public bool IsFavourite => data.IsFavourite;

    /// <inheritdoc />
    public bool IsSharedHouse => data.IsSharedHouse;

    /// <inheritdoc />
    public bool IsApartment => data.IsApartment;

    /// <inheritdoc />
    public RowRef<Lumina.Excel.Sheets.Aetheryte> AetheryteData => LuminaUtils.CreateRef<Lumina.Excel.Sheets.Aetheryte>(this.AetheryteId);
}
