using System.Numerics;

using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.IoC.Internal;
using Dalamud.Services.ObjectTable;
using Dalamud.Services.PlayerState;
using Dalamud.Utility;

using Lumina.Text.ReadOnly;

namespace Dalamud.Game.ClientState.Objects.Types;

/// <summary>
/// This class represents a GameObject in FFXIV.
/// </summary>
internal partial class GameObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameObject"/> class.
    /// </summary>
    /// <param name="address">The address of this game object in memory.</param>
    internal GameObject(IntPtr address)
    {
        this.Address = address;
    }

    /// <summary>
    /// Gets or sets the address of the game object in memory.
    /// </summary>
    public IntPtr Address { get; internal set; }

    /// <summary>
    /// This allows you to <c>if (obj) {...}</c> to check for validity.
    /// </summary>
    /// <param name="gameObject">The actor to check.</param>
    /// <returns>True or false.</returns>
    public static implicit operator bool(GameObject? gameObject) => IsValid(gameObject);

    public static bool operator ==(GameObject? gameObject1, GameObject? gameObject2)
    {
        // Using == results in a stack overflow.
        if (gameObject1 is null || gameObject2 is null)
            return Equals(gameObject1, gameObject2);

        return gameObject1.Equals(gameObject2);
    }

    public static bool operator !=(GameObject? actor1, GameObject? actor2) => !(actor1 == actor2);

    /// <summary>
    /// Gets a value indicating whether this actor is still valid in memory.
    /// </summary>
    /// <param name="actor">The actor to check.</param>
    /// <returns>True or false.</returns>
    public static bool IsValid(IGameObject? actor)
    {
        if (actor == null)
            return false;

        return Service<PlayerState>.Get().IsLoaded;
    }

    /// <summary>
    /// Gets a value indicating whether this actor is still valid in memory.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsValid() => IsValid(this);

    /// <inheritdoc/>
    bool IEquatable<IGameObject>.Equals(IGameObject other) => this.GameObjectId == other?.GameObjectId;

    /// <inheritdoc/>
    public override bool Equals(object obj) => ((IEquatable<IGameObject>)this).Equals(obj as IGameObject);

    /// <inheritdoc/>
    public override int GetHashCode() => this.GameObjectId.GetHashCode();
}

/// <summary>
/// This class represents a basic actor (GameObject) in FFXIV.
/// </summary>
internal unsafe partial class GameObject : IGameObject
{
    /// <inheritdoc/>
    public ReadOnlySeStringSpan Name => this.Struct->GetName().AsSpan();

    /// <inheritdoc/>
    public ulong GameObjectId => this.Struct->GetGameObjectId();

    /// <inheritdoc/>
    public uint EntityId => this.Struct->EntityId;

    /// <inheritdoc/>
    public uint DataId => this.Struct->BaseId;

    /// <inheritdoc/>
    public uint BaseId => this.Struct->BaseId;

    /// <inheritdoc/>
    public uint OwnerId => this.Struct->OwnerId;

    /// <inheritdoc/>
    public ushort ObjectIndex => this.Struct->ObjectIndex;

    /// <inheritdoc/>
    public ObjectKind ObjectKind => (ObjectKind)this.Struct->ObjectKind;

    /// <inheritdoc/>
    public byte SubKind => this.Struct->SubKind;

    /// <inheritdoc/>
    [Obsolete]
    public byte YalmDistanceX => this.Struct->YalmDistanceFromPlayerX;

    /// <inheritdoc/>
    [Obsolete]
    public byte YalmDistanceZ => this.Struct->YalmDistanceFromPlayerZ;

    /// <inheritdoc/>
    public byte CurrentDistance => this.Struct->CurrentDistance;

    /// <inheritdoc/>
    public byte NextDistance => this.Struct->NextDistance;

    /// <inheritdoc/>
    public bool IsDead => this.Struct->IsDead();

    /// <inheritdoc/>
    public bool IsTargetable => this.Struct->GetIsTargetable();

    /// <inheritdoc/>
    public Vector3 Position => new(this.Struct->Position.X, this.Struct->Position.Y, this.Struct->Position.Z);

    /// <inheritdoc/>
    public float Rotation => this.Struct->Rotation;

    /// <inheritdoc/>
    public float HitboxRadius => this.Struct->HitboxRadius;

    /// <inheritdoc/>
    public virtual ulong TargetObjectId => 0;

    /// <inheritdoc/>
    // TODO: Fix for non-networked GameObjects
    public virtual IGameObject? TargetObject => Service<ObjectTable>.Get().SearchById(this.TargetObjectId);

    /// <summary>
    /// Gets the underlying structure.
    /// </summary>
    protected internal FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* Struct
    {
        get
        {
            ThreadSafety.DevModeAssertMainThread();
            return (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)this.Address;
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"{this.GameObjectId:X}({this.Name.ToString()} - {this.ObjectKind}) at {this.Address:X}";
}
