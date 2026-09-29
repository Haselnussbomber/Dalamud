using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;

namespace Dalamud.Game.ClientState.Objects.Types;

/// <summary>
/// This class represents the battle characters.
/// </summary>
internal unsafe class BattleChara : Character, IBattleChara
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BattleChara"/> class.
    /// This represents a battle character.
    /// </summary>
    /// <param name="address">The address of this character in memory.</param>
    internal BattleChara(IntPtr address)
        : base(address)
    {
    }

    /// <inheritdoc/>
    public StatusList StatusList => new(this.Struct->GetStatusManager());

    /// <inheritdoc/>
    public bool IsCasting => this.Struct->GetCastInfo()->IsCasting;

    /// <inheritdoc/>
    public bool IsCastInterruptible => this.Struct->GetCastInfo()->Interruptible;

    /// <inheritdoc/>
    public byte CastActionType => (byte)this.Struct->GetCastInfo()->ActionType;

    /// <inheritdoc/>
    public uint CastActionId => this.Struct->GetCastInfo()->ActionId;

    /// <inheritdoc/>
    public ulong CastTargetObjectId => this.Struct->GetCastInfo()->TargetId;

    /// <inheritdoc/>
    public float CurrentCastTime => this.Struct->GetCastInfo()->CurrentCastTime;

    /// <inheritdoc/>
    public float BaseCastTime => this.Struct->GetCastInfo()->BaseCastTime;

    /// <inheritdoc/>
    public float TotalCastTime => this.Struct->GetCastInfo()->TotalCastTime;

    /// <summary>
    /// Gets the underlying structure.
    /// </summary>
    protected internal new FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* Struct
    {
        get
        {
            ThreadSafety.DevModeAssertMainThread();
            return (FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara*)this.Address;
        }
    }
}
