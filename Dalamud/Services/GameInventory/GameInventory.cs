using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Dalamud.Game.Enums;
using Dalamud.IoC.Internal;
using Dalamud.Services.GameInventory.InventoryEventArgTypes;

namespace Dalamud.Services.GameInventory;

/// <summary>
/// This class provides events for the players in-game inventory.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal sealed class GameInventory : IInternalDisposableService
{
    /// <summary>
    /// Lists excluded InventoryTypes that are not supported by this service.
    /// </summary>
    internal static readonly GameInventoryType[] ExcludedInventoryTypes = [
        GameInventoryType.Cosmopouch1,
        GameInventoryType.Cosmopouch2
    ];

    private readonly List<GameInventoryPluginScoped> subscribersPendingChange = [];
    private readonly List<GameInventoryPluginScoped> subscribers = [];
    private readonly Lock subscribersLock = new();

    private readonly List<InventoryItemAddedArgs> addedEvents = [];
    private readonly List<InventoryItemRemovedArgs> removedEvents = [];
    private readonly List<InventoryItemChangedArgs> changedEvents = [];
    private readonly List<InventoryItemMovedArgs> movedEvents = [];
    private readonly List<InventoryItemSplitArgs> splitEvents = [];
    private readonly List<InventoryItemMergedArgs> mergedEvents = [];

    [ServiceManager.ServiceDependency]
    private readonly Framework.Framework framework = Service<Framework.Framework>.Get();

    [ServiceManager.ServiceDependency]
    private readonly GameGui.GameGui gameGui = Service<GameGui.GameGui>.Get();

    private readonly GameInventoryType[] inventoryTypes;
    private readonly GameInventoryItem[]?[] inventoryItems;

    private bool subscribersChanged;
    private bool inventoriesMightBeChanged;

    [ServiceManager.ServiceConstructor]
    private GameInventory()
    {
        this.inventoryTypes = [.. Enum.GetValues<GameInventoryType>().Where(static type => !ExcludedInventoryTypes.Contains(type))];
        this.inventoryItems = new GameInventoryItem[this.inventoryTypes.Length][];

        this.gameGui.AgentUpdate += this.OnAgentUpdate;
    }

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        using var scope = this.subscribersLock.EnterScope();

        this.subscribers.Clear();
        this.subscribersPendingChange.Clear();
        this.subscribersChanged = false;
        this.framework.Update -= this.OnFrameworkUpdate;
        this.gameGui.AgentUpdate -= this.OnAgentUpdate;
    }

    /// <summary>
    /// Subscribe to events.
    /// </summary>
    /// <param name="s">The event target.</param>
    public void Subscribe(GameInventoryPluginScoped s)
    {
        using var scope = this.subscribersLock.EnterScope();

        this.subscribersPendingChange.Add(s);
        this.subscribersChanged = true;

        if (this.subscribersPendingChange.Count == 1)
        {
            this.inventoriesMightBeChanged = true;
            this.framework.Update += this.OnFrameworkUpdate;
        }
    }

    /// <summary>
    /// Unsubscribe from events.
    /// </summary>
    /// <param name="s">The event target.</param>
    public void Unsubscribe(GameInventoryPluginScoped s)
    {
        using var scope = this.subscribersLock.EnterScope();

        if (!this.subscribersPendingChange.Remove(s))
            return;

        this.subscribersChanged = true;

        if (this.subscribersPendingChange.Count == 0)
            this.framework.Update -= this.OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(IFramework framework1)
    {
        if (!this.inventoriesMightBeChanged)
            return;

        this.inventoriesMightBeChanged = false;

        for (var i = 0; i < this.inventoryTypes.Length; i++)
        {
            var newItems = GameInventoryItem.GetReadOnlySpanOfInventory(this.inventoryTypes[i]);
            if (newItems.IsEmpty)
                continue;

            // Assumption: newItems is sorted by slots, and the last item has the highest slot number.
            var oldItems = this.inventoryItems[i] ??= this.CreateItemsArray(newItems[^1].InternalItem.Slot + 1);

            foreach (ref readonly var newItem in newItems)
            {
                ref var oldItem = ref oldItems[newItem.InternalItem.Slot];

                if (oldItem.IsEmpty)
                {
                    if (!newItem.IsEmpty)
                    {
                        this.addedEvents.Add(new(newItem));
                        oldItem = newItem;
                    }
                }
                else
                {
                    if (newItem.IsEmpty)
                    {
                        this.removedEvents.Add(new(oldItem));
                        oldItem = newItem;
                    }
                    else if (!oldItem.Equals(newItem))
                    {
                        this.changedEvents.Add(new(oldItem, newItem));
                        oldItem = newItem;
                    }
                }
            }
        }

        // Was there any change? If not, stop further processing.
        // Note that only these three are checked; the rest will be populated after this check.
        if (this.addedEvents.Count == 0 && this.removedEvents.Count == 0 && this.changedEvents.Count == 0)
            return;

        // Make a copy of subscribers, to accommodate self removal during the loop.
        if (this.subscribersChanged)
        {
            bool isNew;

            using (this.subscribersLock.EnterScope())
            {
                isNew = this.subscribersPendingChange.Count != 0 && this.subscribers.Count == 0;
                this.subscribers.Clear();
                this.subscribers.AddRange(this.subscribersPendingChange);
                this.subscribersChanged = false;
            }

            // Is this the first time (resuming) scanning for changes? Then discard the "changes".
            if (isNew)
            {
                this.addedEvents.Clear();
                this.removedEvents.Clear();
                this.changedEvents.Clear();
                return;
            }
        }

        // Broadcast InventoryChangedRaw.
        // Same reason with the above on why are there 3 lists of events involved.
        var allRawEventsCollection = new DeferredReadOnlyCollection<InventoryEventArgs>(
            this.addedEvents.Count +
            this.removedEvents.Count +
            this.changedEvents.Count,
            () => Array.Empty<InventoryEventArgs>()
                       .Concat(this.addedEvents)
                       .Concat(this.removedEvents)
                       .Concat(this.changedEvents));
        foreach (var s in this.subscribers)
            s.InvokeChangedRaw(allRawEventsCollection);

        // Resolve moved items, from 1 added + 1 removed event.
        for (var iAdded = this.addedEvents.Count - 1; iAdded >= 0; --iAdded)
        {
            var added = this.addedEvents[iAdded];
            for (var iRemoved = this.removedEvents.Count - 1; iRemoved >= 0; --iRemoved)
            {
                var removed = this.removedEvents[iRemoved];
                if (added.Item.ItemId != removed.Item.ItemId)
                    continue;

                this.movedEvents.Add(new(removed, added));

                // Remove the reinterpreted entries.
                this.addedEvents.RemoveAt(iAdded);
                this.removedEvents.RemoveAt(iRemoved);
                break;
            }
        }

        // Resolve moved items, from 2 changed events.
        for (var i = this.changedEvents.Count - 1; i >= 0; --i)
        {
            var e1 = this.changedEvents[i];
            for (var j = i - 1; j >= 0; --j)
            {
                var e2 = this.changedEvents[j];
                if (e1.Item.ItemId != e2.OldItemState.ItemId || e1.OldItemState.ItemId != e2.Item.ItemId)
                    continue;

                // Move happened, and e2 has an item.
                if (!e2.Item.IsEmpty)
                    this.movedEvents.Add(new(e1, e2));

                // Move happened, and e1 has an item.
                if (!e1.Item.IsEmpty)
                    this.movedEvents.Add(new(e2, e1));

                // Remove the reinterpreted entries. Note that i > j.
                this.changedEvents.RemoveAt(i);
                this.changedEvents.RemoveAt(j);

                // We've removed two. Adjust the outer counter.
                --i;
                break;
            }
        }

        // Resolve split items, from 1 added + 1 changed event.
        for (var iAdded = this.addedEvents.Count - 1; iAdded >= 0; --iAdded)
        {
            var added = this.addedEvents[iAdded];
            for (var iChanged = this.changedEvents.Count - 1; iChanged >= 0; --iChanged)
            {
                var changed = this.changedEvents[iChanged];
                if (added.Item.ItemId != changed.Item.ItemId || added.Item.ItemId != changed.OldItemState.ItemId)
                    continue;

                this.splitEvents.Add(new(changed, added));

                // Remove the reinterpreted entries.
                this.addedEvents.RemoveAt(iAdded);
                this.changedEvents.RemoveAt(iChanged);
                break;
            }
        }

        // Resolve merged items, from 1 removed + 1 changed event.
        for (var iRemoved = this.removedEvents.Count - 1; iRemoved >= 0; --iRemoved)
        {
            var removed = this.removedEvents[iRemoved];
            for (var iChanged = this.changedEvents.Count - 1; iChanged >= 0; --iChanged)
            {
                var changed = this.changedEvents[iChanged];
                if (removed.Item.ItemId != changed.Item.ItemId || removed.Item.ItemId != changed.OldItemState.ItemId)
                    continue;

                this.mergedEvents.Add(new(removed, changed));

                // Remove the reinterpreted entries.
                this.removedEvents.RemoveAt(iRemoved);
                this.changedEvents.RemoveAt(iChanged);
                break;
            }
        }

        // Create a collection view of all events.
        var allEventsCollection = new DeferredReadOnlyCollection<InventoryEventArgs>(
            this.addedEvents.Count +
            this.removedEvents.Count +
            this.changedEvents.Count +
            this.movedEvents.Count +
            this.splitEvents.Count +
            this.mergedEvents.Count,
            () => Array.Empty<InventoryEventArgs>()
                       .Concat(this.addedEvents)
                       .Concat(this.removedEvents)
                       .Concat(this.changedEvents)
                       .Concat(this.movedEvents)
                       .Concat(this.splitEvents)
                       .Concat(this.mergedEvents));

        // Broadcast the rest.
        foreach (var s in this.subscribers)
        {
            s.InvokeChanged(allEventsCollection);
            s.Invoke(this.addedEvents);
            s.Invoke(this.removedEvents);
            s.Invoke(this.changedEvents);
            s.Invoke(this.movedEvents);
            s.Invoke(this.splitEvents);
            s.Invoke(this.mergedEvents);
        }

        // We're done using the lists. Clean them up.
        this.addedEvents.Clear();
        this.removedEvents.Clear();
        this.changedEvents.Clear();
        this.movedEvents.Clear();
        this.splitEvents.Clear();
        this.mergedEvents.Clear();
    }

    private GameInventoryItem[] CreateItemsArray(int length)
    {
        var items = new GameInventoryItem[length];
        foreach (ref var item in items.AsSpan())
            item = new();
        return items;
    }

    private void OnAgentUpdate(AgentUpdateFlag agentUpdateFlag)
    {
        this.inventoriesMightBeChanged |= true;
    }

    /// <summary>
    /// A <see cref="IReadOnlyCollection{T}"/> view of <see cref="IEnumerable{T}"/>, so that the number of items
    /// contained within can be known in advance, and it can be enumerated multiple times.
    /// </summary>
    /// <typeparam name="T">The type of elements being enumerated.</typeparam>
    private class DeferredReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private readonly Func<IEnumerable<T>> enumerableGenerator;

        public DeferredReadOnlyCollection(int count, Func<IEnumerable<T>> enumerableGenerator)
        {
            this.enumerableGenerator = enumerableGenerator;
            this.Count = count;
        }

        public int Count { get; }

        public IEnumerator<T> GetEnumerator() => this.enumerableGenerator().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.enumerableGenerator().GetEnumerator();
    }
}
