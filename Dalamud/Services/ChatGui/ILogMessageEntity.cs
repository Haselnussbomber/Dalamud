using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ChatGui;

/// <summary>
/// Interface representing an entity related to a log message.
/// </summary>
public interface ILogMessageEntity : IEquatable<ILogMessageEntity>
{
    /// <summary>
    /// Gets the name of this entity.
    /// </summary>
    ReadOnlySeString Name { get; }

    /// <summary>
    /// Gets the ID of the homeworld of this entity, if it is a player.
    /// </summary>
    ushort HomeWorldId { get; }

    /// <summary>
    /// Gets the homeworld of this entity, if it is a player.
    /// </summary>
    RowRef<World> HomeWorld { get; }

    /// <summary>
    /// Gets the ObjStr ID of this entity, if not a player. See <seealso cref="ISeStringEvaluator.EvaluateObjStr"/>.
    /// </summary>
    uint ObjStrId { get; }

    /// <summary>
    /// Gets a value indicating whether this entity is a player.
    /// </summary>
    bool IsPlayer { get; }
}
