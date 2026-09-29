using System.Collections.Generic;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ContextMenu;

/// <summary>
/// An interface representing the callback args used when a menu item is clicked.
/// </summary>
public interface IMenuItemClickedArgs : IMenuArgs
{
    /// <summary>
    /// Opens a submenu with the given name and items.
    /// </summary>
    /// <param name="name">The name of the submenu, displayed at the top.</param>
    /// <param name="items">The items to display in the submenu.</param>
    void OpenSubmenu(ReadOnlySeString name, IReadOnlyList<IMenuItem> items);

    /// <summary>
    /// Opens a submenu with the given items.
    /// </summary>
    /// <param name="items">The items to display in the submenu.</param>
    void OpenSubmenu(IReadOnlyList<IMenuItem> items);
}
