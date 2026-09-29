using Dalamud.Game.ClientState.Customize;
using Dalamud.Game.ClientState.Objects.Enums;

using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace Dalamud.Game.ClientState.Objects.Types;

/// <summary>
/// Interface representing a character.
/// </summary>
public interface ICharacter : IGameObject
{
    /// <summary>
    /// Gets the current HP of this character.
    /// </summary>
    uint CurrentHp { get; }

    /// <summary>
    /// Gets the maximum HP of this character.
    /// </summary>
    uint MaxHp { get; }

    /// <summary>
    /// Gets the current MP of this character.
    /// </summary>
    uint CurrentMp { get; }

    /// <summary>
    /// Gets the maximum MP of this character.
    /// </summary>
    uint MaxMp { get; }

    /// <summary>
    /// Gets the current GP of this character.
    /// </summary>
    uint CurrentGp { get; }

    /// <summary>
    /// Gets the maximum GP of this character.
    /// </summary>
    uint MaxGp { get; }

    /// <summary>
    /// Gets the current CP of this character.
    /// </summary>
    uint CurrentCp { get; }

    /// <summary>
    /// Gets the maximum CP of this character.
    /// </summary>
    uint MaxCp { get; }

    /// <summary>
    /// Gets the shield percentage of this character.
    /// </summary>
    byte ShieldPercentage { get; }

    /// <summary>
    /// Gets the ClassJob of this character.
    /// </summary>
    RowRef<ClassJob> ClassJob { get; }

    /// <summary>
    /// Gets the level of this character.
    /// </summary>
    byte Level { get; }

    /// <summary>
    /// Gets a byte array describing the visual appearance of this character.
    /// Indexed by <see cref="CustomizeIndex"/>.
    /// </summary>
    Span<byte> Customize { get; }

    /// <summary>
    /// Gets the underlying CustomizeData struct for this character.
    /// </summary>
    ICustomizeData CustomizeData { get; }

    /// <summary>
    /// Gets the Free Company tag of this character.
    /// </summary>
    string CompanyTag { get; }

    /// <summary>
    /// Gets the name ID of the character.
    /// </summary>
    uint NameId { get; }

    /// <summary>
    /// Gets the current online status of the character.
    /// </summary>
    RowRef<OnlineStatus> OnlineStatus { get; }

    /// <summary>
    /// Gets the status flags.
    /// </summary>
    StatusFlags StatusFlags { get; }

    /// <summary>
    /// Gets the current mount for this character. Will be <c>null</c> if the character doesn't have a mount.
    /// </summary>
    RowRef<Mount>? CurrentMount { get; }

    /// <summary>
    /// Gets the current minion summoned for this character. Will be <c>null</c> if the character doesn't have a minion.
    /// This method *will* return information about a spawned (but invisible) minion, e.g. if the character is riding a
    /// mount.
    /// </summary>
    RowRef<Companion>? CurrentMinion { get; }
}
