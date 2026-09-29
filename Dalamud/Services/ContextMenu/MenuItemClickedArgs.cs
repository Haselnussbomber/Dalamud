using System.Collections.Generic;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ContextMenu;

/// <summary>
/// Callback args used when a menu item is clicked.
/// </summary>
internal sealed unsafe class MenuItemClickedArgs : MenuArgs, IMenuItemClickedArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemClickedArgs"/> class.
    /// </summary>
    /// <param name="openSubmenu">Callback for opening a submenu.</param>
    /// <param name="addon">Addon associated with the context menu.</param>
    /// <param name="agent">Agent associated with the context menu.</param>
    /// <param name="type">The type of context menu.</param>
    /// <param name="eventInterfaces">List of AtkEventInterfaces associated with the context menu.</param>
    internal MenuItemClickedArgs(Action<ReadOnlySeString, IReadOnlyList<IMenuItem>> openSubmenu, AtkUnitBase* addon, AgentInterface* agent, ContextMenuType type, IReadOnlySet<nint> eventInterfaces)
        : base(addon, agent, type, eventInterfaces)
    {
        this.OnOpenSubmenu = openSubmenu;
    }

    private Action<ReadOnlySeString, IReadOnlyList<IMenuItem>> OnOpenSubmenu { get; }

    /// <inheritdoc/>
    public void OpenSubmenu(ReadOnlySeString name, IReadOnlyList<IMenuItem> items) =>
        this.OnOpenSubmenu(name, items);

    /// <inheritdoc/>
    public void OpenSubmenu(IReadOnlyList<IMenuItem> items) =>
        this.OnOpenSubmenu(default, items);
}
