using System.Collections.Generic;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Dalamud.Services.ContextMenu;

/// <summary>
/// Interface representing a context menus args.
/// </summary>
public interface IMenuArgs
{
    /// <summary>
    /// Gets a list of AtkEventInterface pointers associated with the context menu.
    /// Only available with <see cref="ContextMenuType.Default"/>.
    /// Almost always an agent pointer. You can use this to find out what type of context menu it is.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the context menu is not a <see cref="ContextMenuType.Default"/>.</exception>
    IReadOnlySet<nint> EventInterfaces { get; }

    /// <summary>
    /// Gets the name of the addon that opened the context menu.
    /// </summary>
    string? AddonName { get; }

    /// <summary>
    /// Gets the memory pointer of the addon that opened the context menu.
    /// </summary>
    nint AddonPtr { get; }

    /// <summary>
    /// Gets the memory pointer of the agent that opened the context menu.
    /// </summary>
    nint AgentPtr { get; }

    /// <summary>
    /// Gets the type of the context menu.
    /// </summary>
    ContextMenuType MenuType { get; }

    /// <summary>
    /// Gets the target info of the context menu. The actual type depends on <see cref="MenuType"/>.
    /// <see cref="ContextMenuType.Default"/> signifies a <see cref="MenuTargetDefault"/>.
    /// <see cref="ContextMenuType.Inventory"/> signifies a <see cref="MenuTargetInventory"/>.
    /// </summary>
    MenuTarget Target { get; }
}
