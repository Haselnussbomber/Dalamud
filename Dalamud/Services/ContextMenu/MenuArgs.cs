using System.Collections.Generic;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Dalamud.Services.ContextMenu;

/// <summary>
/// Base class for <see cref="IContextMenu"/> menu args.
/// </summary>
internal abstract unsafe class MenuArgs : IMenuArgs
{
    private IReadOnlySet<nint>? eventInterfaces;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuArgs"/> class.
    /// </summary>
    /// <param name="addon">Addon associated with the context menu.</param>
    /// <param name="agent">Agent associated with the context menu.</param>
    /// <param name="type">The type of context menu.</param>
    /// <param name="eventInterfaces">List of AtkEventInterfaces associated with the context menu.</param>
    protected internal MenuArgs(AtkUnitBase* addon, AgentInterface* agent, ContextMenuType type, IReadOnlySet<nint>? eventInterfaces)
    {
        this.AddonName = addon != null ? addon->NameString : null;
        this.AddonPtr = (nint)addon;
        this.AgentPtr = (nint)agent;
        this.MenuType = type;
        this.eventInterfaces = eventInterfaces;
        this.Target = type switch
        {
            ContextMenuType.Default => new MenuTargetDefault((AgentContext*)agent),
            ContextMenuType.Inventory => new MenuTargetInventory((AgentInventoryContext*)agent),
            _ => throw new ArgumentException("Invalid context menu type", nameof(type)),
        };
    }

    /// <inheritdoc/>
    public string? AddonName { get; }

    /// <inheritdoc/>
    public nint AddonPtr { get; }

    /// <inheritdoc/>
    public nint AgentPtr { get; }

    /// <inheritdoc/>
    public ContextMenuType MenuType { get; }

    /// <inheritdoc/>
    public MenuTarget Target { get; }

    /// <inheritdoc/>
    public IReadOnlySet<nint> EventInterfaces
    {
        get
        {
            if (this.MenuType is ContextMenuType.Default)
            {
                return this.eventInterfaces ?? new HashSet<nint>();
            }
            else
            {
                throw new InvalidOperationException("Not a default context menu");
            }
        }
    }
}
