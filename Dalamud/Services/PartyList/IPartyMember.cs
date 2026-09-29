using System.Numerics;

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;

using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace Dalamud.Services.PartyList;

/// <summary>
/// Interface representing a party member.
/// </summary>
public interface IPartyMember : IEquatable<IPartyMember>
{
    /// <summary>
    /// Gets the address of this party member in memory.
    /// </summary>
    nint Address { get; }

    /// <summary>
    /// Gets a list of buffs or debuffs applied to this party member.
    /// </summary>
    StatusList Statuses { get; }

    /// <summary>
    /// Gets the position of the party member.
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// Gets the content ID of the party member.
    /// </summary>
    ulong ContentId { get; }

    /// <summary>
    /// Gets the actor ID of this party member.
    /// </summary>
    [Obsolete("Renamed to EntityId")]
    uint ObjectId { get; }

    /// <summary>
    /// Gets the entity ID of this party member.
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
    /// Gets the current HP of this party member.
    /// </summary>
    uint CurrentHP { get; }

    /// <summary>
    /// Gets the maximum HP of this party member.
    /// </summary>
    uint MaxHP { get; }

    /// <summary>
    /// Gets the current MP of this party member.
    /// </summary>
    ushort CurrentMP { get; }

    /// <summary>
    /// Gets the maximum MP of this party member.
    /// </summary>
    ushort MaxMP { get; }

    /// <summary>
    /// Gets the territory this party member is located in.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.TerritoryType> Territory { get; }

    /// <summary>
    /// Gets the World this party member resides in.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.World> World { get; }

    /// <summary>
    /// Gets the displayname of this party member.
    /// </summary>
    ReadOnlySeStringSpan Name { get; }

    /// <summary>
    /// Gets the sex of this party member.
    /// </summary>
    byte Sex { get; }

    /// <summary>
    /// Gets the classjob of this party member.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.ClassJob> ClassJob { get; }

    /// <summary>
    /// Gets the level of this party member.
    /// </summary>
    byte Level { get; }
}
