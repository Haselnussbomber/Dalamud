using Dalamud.Game.ClientState.Statuses;

namespace Dalamud.Game.ClientState.Objects.Types;

/// <summary>
/// Interface representing a battle character.
/// </summary>
public interface IBattleChara : ICharacter
{
    /// <summary>
    /// Gets the current status effects.
    /// </summary>
    StatusList StatusList { get; }

    /// <summary>
    /// Gets a value indicating whether the chara is currently casting.
    /// </summary>
    bool IsCasting { get; }

    /// <summary>
    /// Gets a value indicating whether the cast is interruptible.
    /// </summary>
    bool IsCastInterruptible { get; }

    /// <summary>
    /// Gets the spell action type of the spell being cast by the actor.
    /// </summary>
    byte CastActionType { get; }

    /// <summary>
    /// Gets the spell action ID of the spell being cast by the actor.
    /// </summary>
    uint CastActionId { get; }

    /// <summary>
    /// Gets the object ID of the target currently being cast at by the chara.
    /// </summary>
    ulong CastTargetObjectId { get; }

    /// <summary>
    /// Gets the current casting time of the spell being cast by the chara.
    /// </summary>
    float CurrentCastTime { get; }

    /// <summary>
    /// Gets the base casting time of the spell being cast by the chara.
    /// </summary>
    /// <remarks>
    /// This can only be a portion of the total cast for some actions.
    /// Use TotalCastTime if you always need the total cast time.
    /// </remarks>
    float BaseCastTime { get; }

    /// <summary>
    /// Gets the <see cref="BaseCastTime"/> plus any adjustments from the game, such as Action offset 2B. Used for display purposes.
    /// </summary>
    float TotalCastTime { get; }
}
