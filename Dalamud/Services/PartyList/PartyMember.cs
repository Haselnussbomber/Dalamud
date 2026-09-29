using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Services.DataManager;
using Dalamud.Utility;

using Lumina.Excel;
using Lumina.Text.ReadOnly;

using CSPartyMember = FFXIVClientStructs.FFXIV.Client.Game.Group.PartyMember;

namespace Dalamud.Services.PartyList;

/// <summary>
/// This struct represents a party member in the group manager.
/// </summary>
/// <param name="address">A pointer to the PartyMember.</param>
internal readonly unsafe struct PartyMember(nint address) : IPartyMember
{
    /// <inheritdoc/>
    public nint Address => address;

    /// <inheritdoc/>
    public StatusList Statuses => new(&this.Struct->StatusManager);

    /// <inheritdoc/>
    public Vector3 Position => this.Struct->Position;

    /// <inheritdoc/>
    public ulong ContentId => this.Struct->ContentId;

    /// <inheritdoc/>
    public uint ObjectId => this.Struct->EntityId;

    /// <inheritdoc/>
    public uint EntityId => this.Struct->EntityId;

    /// <inheritdoc/>
    public IGameObject? GameObject => Service<ObjectTable.ObjectTable>.Get().SearchById(this.EntityId);

    /// <inheritdoc/>
    public uint CurrentHP => this.Struct->CurrentHP;

    /// <inheritdoc/>
    public uint MaxHP => this.Struct->MaxHP;

    /// <inheritdoc/>
    public ushort CurrentMP => this.Struct->CurrentMP;

    /// <inheritdoc/>
    public ushort MaxMP => this.Struct->MaxMP;

    /// <inheritdoc/>
    public RowRef<Lumina.Excel.Sheets.TerritoryType> Territory => LuminaUtils.CreateRef<Lumina.Excel.Sheets.TerritoryType>(this.Struct->TerritoryType);

    /// <inheritdoc/>
    public RowRef<Lumina.Excel.Sheets.World> World => LuminaUtils.CreateRef<Lumina.Excel.Sheets.World>(this.Struct->HomeWorld);

    /// <inheritdoc/>
    public ReadOnlySeStringSpan Name => this.Struct->Name;

    /// <inheritdoc/>
    public byte Sex => this.Struct->Sex;

    /// <inheritdoc/>
    public RowRef<Lumina.Excel.Sheets.ClassJob> ClassJob => LuminaUtils.CreateRef<Lumina.Excel.Sheets.ClassJob>(this.Struct->ClassJob);

    /// <inheritdoc/>
    public byte Level => this.Struct->Level;

    /// <summary>
    /// Gets the underlying structure.
    /// </summary>
    internal CSPartyMember* Struct
    {
        get
        {
            ThreadSafety.DevModeAssertMainThread();
            return (CSPartyMember*)address;
        }
    }

    public static bool operator ==(PartyMember x, PartyMember y) => x.Equals(y);

    public static bool operator !=(PartyMember x, PartyMember y) => !(x == y);

    /// <inheritdoc/>
    public bool Equals(IPartyMember? other)
    {
        return this.EntityId == other.EntityId;
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is PartyMember fate && this.Equals(fate);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.EntityId.GetHashCode();
    }
}
