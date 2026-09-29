using System.Collections.Generic;
using System.Threading;

using Dalamud.IoC;
using Dalamud.IoC.Internal;

namespace Dalamud.Services.ContextMenu;

/// <summary>
/// Plugin-scoped version of a <see cref="ContextMenu"/> service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IContextMenu>]
#pragma warning restore SA1015
internal class ContextMenuPluginScoped : IInternalDisposableService, IContextMenu
{
    [ServiceManager.ServiceDependency]
    private readonly ContextMenu parentService = Service<ContextMenu>.Get();

    private ContextMenuPluginScoped()
    {
        this.parentService.MenuOpened += this.OnMenuOpenedForward;
    }

    /// <inheritdoc/>
    public event IContextMenu.MenuOpenedDelegate? MenuOpened;

    private Dictionary<ContextMenuType, List<IMenuItem>> MenuItems { get; } = [];

    private Lock MenuItemsLock { get; } = new();

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.parentService.MenuOpened -= this.OnMenuOpenedForward;

        this.MenuOpened = null;

        using var scope = this.MenuItemsLock.EnterScope();

        foreach (var (menuType, items) in this.MenuItems)
        {
            foreach (var item in items)
                this.parentService.RemoveMenuItem(menuType, item);
        }
    }

    /// <inheritdoc/>
    public void AddMenuItem(ContextMenuType menuType, IMenuItem item)
    {
        using var scope = this.MenuItemsLock.EnterScope();

        if (!this.MenuItems.TryGetValue(menuType, out var items))
            this.MenuItems.TryAdd(menuType, items = []);

        items.Add(item);

        this.parentService.AddMenuItem(menuType, item);
    }

    /// <inheritdoc/>
    public bool RemoveMenuItem(ContextMenuType menuType, IMenuItem item)
    {
        using var scope = this.MenuItemsLock.EnterScope();

        if (this.MenuItems.TryGetValue(menuType, out var items))
            items.Remove(item);

        return this.parentService.RemoveMenuItem(menuType, item);
    }

    private void OnMenuOpenedForward(IMenuOpenedArgs args) =>
        this.MenuOpened?.Invoke(args);
}
