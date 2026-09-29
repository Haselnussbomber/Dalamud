using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using Dalamud.Services.DataManager;
using Dalamud.Utility;

using Lumina.Excel;
using Lumina.Text.ReadOnly;

using CSFateContext = FFXIVClientStructs.FFXIV.Client.Game.Fate.FateContext;

namespace Dalamud.Services.FateTable;

/// <summary>
/// This struct represents a Fate.
/// </summary>
/// <param name="ptr">A pointer to the FateContext.</param>
internal readonly unsafe struct Fate(CSFateContext* ptr) : IFate
{
    /// <inheritdoc />
    public nint Address => (nint)ptr;

    /// <inheritdoc/>
    public ushort FateId => ptr->FateId;

    /// <inheritdoc/>
    public RowRef<Lumina.Excel.Sheets.Fate> GameData => LuminaUtils.CreateRef<Lumina.Excel.Sheets.Fate>(this.FateId);

    /// <inheritdoc/>
    public int StartTimeEpoch => ptr->StartTimeEpoch;

    /// <inheritdoc/>
    public short Duration => ptr->Duration;

    /// <inheritdoc/>
    public long TimeRemaining => this.StartTimeEpoch + this.Duration - DateTimeOffset.Now.ToUnixTimeSeconds();

    /// <inheritdoc/>
    public ReadOnlySeStringSpan Name => ptr->Name.AsReadOnlySeStringSpan();

    /// <inheritdoc/>
    public ReadOnlySeStringSpan Description => ptr->Description.AsReadOnlySeStringSpan();

    /// <inheritdoc/>
    public ReadOnlySeStringSpan Objective => ptr->Objective.AsReadOnlySeStringSpan();

    /// <inheritdoc/>
    public FateState State => (FateState)ptr->State;

    /// <inheritdoc/>
    public byte HandInCount => ptr->HandInCount;

    /// <inheritdoc/>
    public byte Progress => ptr->Progress;

    /// <inheritdoc/>
    public bool HasBonus => ptr->IsBonus;

    /// <inheritdoc/>
    public uint IconId => ptr->IconId;

    /// <inheritdoc/>
    public byte Level => ptr->Level;

    /// <inheritdoc/>
    public byte MaxLevel => ptr->MaxLevel;

    /// <inheritdoc/>
    public Vector3 Position => ptr->Location;

    /// <inheritdoc/>
    public float Radius => ptr->Radius;

    /// <inheritdoc/>
    public uint MapIconId => ptr->MapIconId;

    /// <summary>
    /// Gets the territory this <see cref="Fate"/> is located in.
    /// </summary>
    public RowRef<Lumina.Excel.Sheets.TerritoryType> TerritoryType => LuminaUtils.CreateRef<Lumina.Excel.Sheets.TerritoryType>(ptr->MapMarkers[0].MapMarkerData.TerritoryTypeId);

    public static bool operator ==(Fate x, Fate y) => x.Equals(y);

    public static bool operator !=(Fate x, Fate y) => !(x == y);

    /// <inheritdoc/>
    public bool Equals(IFate? other)
    {
        return this.FateId == other.FateId;
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Fate fate && this.Equals(fate);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.FateId.GetHashCode();
    }
}
