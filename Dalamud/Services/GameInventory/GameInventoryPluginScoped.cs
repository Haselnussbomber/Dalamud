using System.Collections.Generic;

using Dalamud.IoC;
using Dalamud.IoC.Internal;
using Dalamud.Logging.Internal;
using Dalamud.Plugin.Internal;
using Dalamud.Services.GameInventory.InventoryEventArgTypes;

namespace Dalamud.Services.GameInventory;

/// <summary>
/// Plugin-scoped version of a GameInventory service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IGameInventory>]
#pragma warning restore SA1015
internal class GameInventoryPluginScoped : IInternalDisposableService, IGameInventory
{
    private static readonly ModuleLog Log = ModuleLog.Create<GameInventoryPluginScoped>();

    [ServiceManager.ServiceDependency]
    private readonly GameInventory gameInventoryService = Service<GameInventory>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameInventoryPluginScoped"/> class.
    /// </summary>
    public GameInventoryPluginScoped() => this.gameInventoryService.Subscribe(this);

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangelogDelegate? InventoryChanged;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangelogDelegate? InventoryChangedRaw;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate? ItemAdded;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate? ItemRemoved;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate? ItemChanged;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate? ItemMoved;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate? ItemSplit;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate? ItemMerged;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate<InventoryItemAddedArgs>? ItemAddedExplicit;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate<InventoryItemRemovedArgs>? ItemRemovedExplicit;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate<InventoryItemChangedArgs>? ItemChangedExplicit;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate<InventoryItemMovedArgs>? ItemMovedExplicit;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate<InventoryItemSplitArgs>? ItemSplitExplicit;

    /// <inheritdoc/>
    public event IGameInventory.InventoryChangedDelegate<InventoryItemMergedArgs>? ItemMergedExplicit;

    /// <inheritdoc/>
    public ReadOnlySpan<GameInventoryItem> GetInventoryItems(GameInventoryType type) => GameInventoryItem.GetReadOnlySpanOfInventory(type);

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.gameInventoryService.Unsubscribe(this);

        this.InventoryChanged = null;
        this.InventoryChangedRaw = null;
        this.ItemAdded = null;
        this.ItemRemoved = null;
        this.ItemChanged = null;
        this.ItemMoved = null;
        this.ItemSplit = null;
        this.ItemMerged = null;
        this.ItemAddedExplicit = null;
        this.ItemRemovedExplicit = null;
        this.ItemChangedExplicit = null;
        this.ItemMovedExplicit = null;
        this.ItemSplitExplicit = null;
        this.ItemMergedExplicit = null;
    }

    /// <summary>
    /// Invoke <see cref="InventoryChanged"/>.
    /// </summary>
    /// <param name="data">The data.</param>
    internal void InvokeChanged(IReadOnlyCollection<InventoryEventArgs> data)
    {
        try
        {
            this.InventoryChanged?.Invoke(data);
        }
        catch (Exception e)
        {
            Log.Error(
                e,
                "[{plugin}] Exception during {argType} callback",
                Service<PluginManager>.GetNullable()?.FindCallingPlugin(new(e))?.Name ?? "(unknown plugin)",
                nameof(this.InventoryChanged));
        }
    }

    /// <summary>
    /// Invoke <see cref="InventoryChangedRaw"/>.
    /// </summary>
    /// <param name="data">The data.</param>
    internal void InvokeChangedRaw(IReadOnlyCollection<InventoryEventArgs> data)
    {
        try
        {
            this.InventoryChangedRaw?.Invoke(data);
        }
        catch (Exception e)
        {
            Log.Error(
                e,
                "[{plugin}] Exception during {argType} callback",
                Service<PluginManager>.GetNullable()?.FindCallingPlugin(new(e))?.Name ?? "(unknown plugin)",
                nameof(this.InventoryChangedRaw));
        }
    }

    // Note below: using List<T> instead of IEnumerable<T>, since List<T> has a specialized lightweight enumerator.

    /// <summary>
    /// Invoke the appropriate event handler.
    /// </summary>
    /// <param name="events">The data.</param>
    internal void Invoke(List<InventoryItemAddedArgs> events) =>
        Invoke(this.ItemAdded, this.ItemAddedExplicit, events);

    /// <summary>
    /// Invoke the appropriate event handler.
    /// </summary>
    /// <param name="events">The data.</param>
    internal void Invoke(List<InventoryItemRemovedArgs> events) =>
        Invoke(this.ItemRemoved, this.ItemRemovedExplicit, events);

    /// <summary>
    /// Invoke the appropriate event handler.
    /// </summary>
    /// <param name="events">The data.</param>
    internal void Invoke(List<InventoryItemChangedArgs> events) =>
        Invoke(this.ItemChanged, this.ItemChangedExplicit, events);

    /// <summary>
    /// Invoke the appropriate event handler.
    /// </summary>
    /// <param name="events">The data.</param>
    internal void Invoke(List<InventoryItemMovedArgs> events) =>
        Invoke(this.ItemMoved, this.ItemMovedExplicit, events);

    /// <summary>
    /// Invoke the appropriate event handler.
    /// </summary>
    /// <param name="events">The data.</param>
    internal void Invoke(List<InventoryItemSplitArgs> events) =>
        Invoke(this.ItemSplit, this.ItemSplitExplicit, events);

    /// <summary>
    /// Invoke the appropriate event handler.
    /// </summary>
    /// <param name="events">The data.</param>
    internal void Invoke(List<InventoryItemMergedArgs> events) =>
        Invoke(this.ItemMerged, this.ItemMergedExplicit, events);

    private static void Invoke<T>(
        IGameInventory.InventoryChangedDelegate? cb,
        IGameInventory.InventoryChangedDelegate<T>? cbt,
        List<T> events) where T : InventoryEventArgs
    {
        foreach (var evt in events)
        {
            try
            {
                cb?.Invoke(evt.Type, evt);
            }
            catch (Exception e)
            {
                Log.Error(
                    e,
                    "[{plugin}] Exception during untyped callback for {evt}",
                    Service<PluginManager>.GetNullable()?.FindCallingPlugin(new(e))?.Name ?? "(unknown plugin)",
                    evt);
            }

            try
            {
                cbt?.Invoke(evt);
            }
            catch (Exception e)
            {
                Log.Error(
                    e,
                    "[{plugin}] Exception during typed callback for {evt}",
                    Service<PluginManager>.GetNullable()?.FindCallingPlugin(new(e))?.Name ?? "(unknown plugin)",
                    evt);
            }
        }
    }
}
