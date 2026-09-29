using Dalamud.Game.ClientState.Objects.Enums;

namespace Dalamud.Game.ClientState.Objects.Types;

/// <summary>
/// A interface that represents a battle NPC.
/// </summary>
public interface IBattleNpc : IBattleChara
{
    /// <summary>
    /// Gets the BattleNpc <see cref="BattleNpcSubKind" /> of this BattleNpc.
    /// </summary>
    BattleNpcSubKind BattleNpcKind { get; }
}
