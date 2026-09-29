using System.Numerics;

using Dalamud.Game.ClientState.Objects.Enums;

using Lumina.Text.ReadOnly;

namespace Dalamud.Game.ClientState.Objects.Types;

/// <summary>
/// Interface representing a game object.
/// </summary>
public interface IGameObject : IEquatable<IGameObject>
{
    /// <summary>
    /// Gets the name of this <see cref="GameObject" />.
    /// </summary>
    ReadOnlySeStringSpan Name { get; }

    /// <summary>
    /// Gets the GameObjectID for this GameObject. The Game Object ID is a globally unique identifier that points to
    /// this specific object. This ID is used to reference specific objects on the local client (e.g. for targeting).
    ///
    /// Not to be confused with <see cref="EntityId"/>.
    /// </summary>
    ulong GameObjectId { get; }

    /// <summary>
    /// Gets the Entity ID for this GameObject. Entity IDs are assigned to networked GameObjects.
    ///
    /// A value of <c>0xE000_0000</c> indicates that this entity is not networked and has specific interactivity rules.
    /// </summary>
    uint EntityId { get; }

    /// <summary>
    /// Gets the data ID for linking to other respective game data.
    /// </summary>
    [Obsolete("Renamed to BaseId")]
    uint DataId { get; }

    /// <summary>
    /// Gets the base ID for linking to other respective game data.
    /// </summary>
    uint BaseId { get; }

    /// <summary>
    /// Gets the ID of this GameObject's owner.
    /// </summary>
    uint OwnerId { get; }

    /// <summary>
    /// Gets the index of this object in the object table.
    /// </summary>
    ushort ObjectIndex { get; }

    /// <summary>
    /// Gets the entity kind of this <see cref="GameObject" />.
    /// See <see cref="ObjectKind">the ObjectKind enum</see> for possible values.
    /// </summary>
    ObjectKind ObjectKind { get; }

    /// <summary>
    /// Gets the sub kind of this Actor.
    /// </summary>
    byte SubKind { get; }

    /// <summary>
    /// Gets the X distance from the local player in yalms.
    /// </summary>
    [Obsolete("Use CurrentDistance.")]
    byte YalmDistanceX { get; }

    /// <summary>
    /// Gets the Y distance from the local player in yalms.
    /// </summary>
    [Obsolete("This property did not represent the Z-axis. It is the next distance value that will replace CurrentDistance on the next update tick. Use CurrentDistance instead.")]
    byte YalmDistanceZ { get; }

    /// <summary>
    /// Gets the current distance from the local player, in yalms.
    /// </summary>
    byte CurrentDistance { get; }

    /// <summary>
    /// Gets the next distance value that will replace <see cref="CurrentDistance"/> on the next update tick.
    /// </summary>
    byte NextDistance { get; }

    /// <summary>
    /// Gets a value indicating whether the object is dead or alive.
    /// </summary>
    bool IsDead { get; }

    /// <summary>
    /// Gets a value indicating whether the object is targetable.
    /// </summary>
    bool IsTargetable { get; }

    /// <summary>
    /// Gets the position of this <see cref="GameObject" />.
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// Gets the rotation of this <see cref="GameObject" />.
    /// This ranges from -pi to pi radians.
    /// </summary>
    float Rotation { get; }

    /// <summary>
    /// Gets the hitbox radius of this <see cref="GameObject" />.
    /// </summary>
    float HitboxRadius { get; }

    /// <summary>
    /// Gets the current target of the game object.
    /// </summary>
    ulong TargetObjectId { get; }

    /// <summary>
    /// Gets the target object of the game object.
    /// </summary>
    /// <remarks>
    /// This iterates the actor table, it should be used with care.
    /// </remarks>
    // TODO: Fix for non-networked GameObjects
    IGameObject? TargetObject { get; }

    /// <summary>
    /// Gets the address of the game object in memory.
    /// </summary>
    IntPtr Address { get; }

    /// <summary>
    /// Gets a value indicating whether this actor is still valid in memory.
    /// </summary>
    /// <returns>True or false.</returns>
    bool IsValid();
}
