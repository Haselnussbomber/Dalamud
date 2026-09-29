using System.Collections.Generic;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Dalamud.Services.ContextMenu;

/// <summary>
/// An interface representing the callback args used when a menu item is opened.
/// </summary>
public interface IMenuOpenedArgs : IMenuArgs
{
    /// <summary>
    /// Adds a custom menu item to the context menu.
    /// </summary>
    /// <param name="item">The menu item to add.</param>
    void AddMenuItem(MenuItem item);
}
