using System.Diagnostics.CodeAnalysis;

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Services.DataManager;

using Lumina.Excel;

using CSBuddyMember = FFXIVClientStructs.FFXIV.Client.Game.UI.Buddy.BuddyMember;

namespace Dalamud.Services.BuddyList;

/// <summary>
/// This struct represents a buddy such as the chocobo companion, summoned pets, squadron groups and trust parties.
/// </summary>
/// <param name="ptr">A pointer to the BuddyMember.</param>
internal readonly unsafe struct BuddyMember(CSBuddyMember* ptr) : IBuddyMember
{
    [ServiceManager.ServiceDependency]
    private readonly ObjectTable.ObjectTable objectTable = Service<ObjectTable.ObjectTable>.Get();

    /// <inheritdoc />
    public nint Address => (nint)ptr;

    /// <inheritdoc />
    public uint ObjectId => this.EntityId;

    /// <inheritdoc />
    public uint EntityId => ptr->EntityId;

    /// <inheritdoc />
    public IGameObject? GameObject => this.objectTable.SearchById(this.EntityId);

    /// <inheritdoc />
    public uint CurrentHP => ptr->CurrentHealth;

    /// <inheritdoc />
    public uint MaxHP => ptr->MaxHealth;

    /// <inheritdoc />
    public uint DataID => ptr->DataId;

    /// <inheritdoc />
    public RowRef<Lumina.Excel.Sheets.Mount> MountData => LuminaUtils.CreateRef<Lumina.Excel.Sheets.Mount>(this.DataID);

    /// <inheritdoc />
    public RowRef<Lumina.Excel.Sheets.Pet> PetData => LuminaUtils.CreateRef<Lumina.Excel.Sheets.Pet>(this.DataID);

    /// <inheritdoc />
    public RowRef<Lumina.Excel.Sheets.DawnGrowMember> TrustData => LuminaUtils.CreateRef<Lumina.Excel.Sheets.DawnGrowMember>(this.DataID);

    public static bool operator ==(BuddyMember x, BuddyMember y) => x.Equals(y);

    public static bool operator !=(BuddyMember x, BuddyMember y) => !(x == y);

    /// <inheritdoc/>
    public bool Equals(IBuddyMember? other)
    {
        return this.EntityId == other.EntityId;
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is BuddyMember fate && this.Equals(fate);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.EntityId.GetHashCode();
    }
}
