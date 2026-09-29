using Dalamud.Game.Gui;

namespace Dalamud.Services.GameGui;

/// <summary>
/// This class represents the hotbar action currently hovered over by the cursor.
/// </summary>
public interface IHoveredAction
{
    /// <summary>
    /// Gets or sets the base action ID.
    /// </summary>
    uint BaseActionId { get; set; }

    /// <summary>
    /// Gets or sets the action ID accounting for automatic upgrades.
    /// </summary>
    uint ActionId { get; set; }

    /// <summary>
    /// Gets or sets the type of action.
    /// </summary>
    DetailKind DetailKind { get; set; }
}
