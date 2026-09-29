using System.Collections.Generic;
using System.Reflection;

using Dalamud.Services.KeyState;

namespace Dalamud.Services.TitleScreenMenu;

/// <summary>
/// A interface representing an entry in the title screen menu.
/// </summary>
public interface ITitleScreenMenuEntry : IReadOnlyTitleScreenMenuEntry, IComparable<TitleScreenMenuEntry>
{
    /// <summary>
    /// Gets or sets a value indicating whether this entry is internal.
    /// </summary>
    bool IsInternal { get; set; }

    /// <summary>
    /// Gets the calling assembly of this entry.
    /// </summary>
    Assembly? CallingAssembly { get; init; }

    /// <summary>
    /// Gets the internal ID of this entry.
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// Gets the keys that have to be pressed to show the menu.
    /// </summary>
    IReadOnlySet<VirtualKey> ShowConditionKeys { get; init; }

    /// <summary>
    /// Determines the displaying condition of this menu entry is met.
    /// </summary>
    /// <returns>True if met.</returns>
    bool IsShowConditionSatisfied();

    /// <summary>
    /// Trigger the action associated with this entry.
    /// </summary>
    void Trigger();
}
