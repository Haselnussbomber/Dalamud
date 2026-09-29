using Dalamud.Game.ClientState.Objects.Types;

using Lumina.Excel;

namespace Dalamud.Services.BuddyList;

/// <summary>
/// Interface representing represents a buddy such as the chocobo companion, summoned pets, squadron groups and trust parties.
/// </summary>
public interface IBuddyMember : IEquatable<IBuddyMember>
{
    /// <summary>
    /// Gets the address of the buddy in memory.
    /// </summary>
    nint Address { get; }

    /// <summary>
    /// Gets the object ID of this buddy.
    /// </summary>
    [Obsolete("Renamed to EntityId")]
    uint ObjectId { get; }

    /// <summary>
    /// Gets the entity ID of this buddy.
    /// </summary>
    uint EntityId { get; }

    /// <summary>
    /// Gets the actor associated with this buddy.
    /// </summary>
    /// <remarks>
    /// This iterates the actor table, it should be used with care.
    /// </remarks>
    IGameObject? GameObject { get; }

    /// <summary>
    /// Gets the current health of this buddy.
    /// </summary>
    uint CurrentHP { get; }

    /// <summary>
    /// Gets the maximum health of this buddy.
    /// </summary>
    uint MaxHP { get; }

    /// <summary>
    /// Gets the data ID of this buddy.
    /// </summary>
    uint DataID { get; }

    /// <summary>
    /// Gets the Mount data related to this buddy. It should only be used with companion buddies.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.Mount> MountData { get; }

    /// <summary>
    /// Gets the Pet data related to this buddy. It should only be used with pet buddies.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.Pet> PetData { get; }

    /// <summary>
    /// Gets the Trust data related to this buddy. It should only be used with battle buddies.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.DawnGrowMember> TrustData { get; }
}
