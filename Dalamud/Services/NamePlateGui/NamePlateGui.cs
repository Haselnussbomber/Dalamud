using System.Collections.Generic;
using System.Runtime.InteropServices;

using Dalamud.Game.ClientState.Objects;
using Dalamud.Hooking;
using Dalamud.Logging.Internal;
using Dalamud.Services.SigScanner;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.NamePlateGui;

/// <summary>
/// Class used to modify the data used when rendering nameplates.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal sealed class NamePlateGui : IInternalDisposableService, INamePlateGui
{
    /// <summary>
    /// An empty null-terminated string pointer allocated in unmanaged memory, used to tag removed fields.
    /// </summary>
    internal static readonly nint EmptyStringPointer = CreateEmptyStringPointer();

    private static readonly ModuleLog Log = ModuleLog.Create<NamePlateGui>();

    [ServiceManager.ServiceDependency]
    private readonly GameGui.GameGui gameGui = Service<GameGui.GameGui>.Get();

    [ServiceManager.ServiceDependency]
    private readonly ObjectTable.ObjectTable objectTable = Service<ObjectTable.ObjectTable>.Get();

    private readonly NamePlateGuiAddressResolver address;

    private readonly Hook<AtkUnitBase.Delegates.OnRequestedUpdate> onRequestedUpdateHook;

    private NamePlateUpdateContext? context;

    private NamePlateUpdateHandler[] updateHandlers = [];

    private bool pendingForceRedraw;

    [ServiceManager.ServiceConstructor]
    private unsafe NamePlateGui(TargetSigScanner sigScanner)
    {
        this.address = new NamePlateGuiAddressResolver();
        this.address.Setup(sigScanner);

        this.onRequestedUpdateHook = Hook<AtkUnitBase.Delegates.OnRequestedUpdate>.FromAddress(
            this.address.OnRequestedUpdate,
            this.OnRequestedUpdateDetour);
        this.onRequestedUpdateHook.Enable();
    }

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? NamePlateUpdate;

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? PostNamePlateUpdate;

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? DataUpdate;

    /// <inheritdoc/>
    public event INamePlateGui.PlateUpdateDelegate? PostDataUpdate;

    /// <inheritdoc/>
    public unsafe void RequestRedraw()
    {
        var addon = this.gameGui.GetAddonByName<AddonNamePlate>("NamePlate"u8);
        if (addon != null)
        {
            AtkStage.Instance()->GetNumberArrayData(NumberArrayType.NamePlate)->UpdateState = 2;
            this.pendingForceRedraw = true;
        }
    }

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.onRequestedUpdateHook.Dispose();
    }

    /// <summary>
    /// Strips the surrounding quotes from a free company tag. If the quotes are not present in the expected location,
    /// no modifications will be made.
    /// </summary>
    /// <param name="text">A quoted free company tag.</param>
    /// <returns>A span containing the free company tag without its surrounding quote characters.</returns>
    internal static ReadOnlySeString StripFreeCompanyTagQuotes(ReadOnlySeString text)
    {
        return text.ReplaceText(" «"u8, default).ReplaceText("»"u8, default);
    }

    /// <summary>
    /// Strips the surrounding quotes from a title. If the quotes are not present in the expected location, no
    /// modifications will be made.
    /// </summary>
    /// <param name="text">A quoted title.</param>
    /// <returns>A span containing the title without its surrounding quote characters.</returns>
    internal static ReadOnlySeString StripTitleQuotes(ReadOnlySeString text)
    {
        return text.ReplaceText("《"u8, default).ReplaceText("》"u8, default);
    }

    private static nint CreateEmptyStringPointer()
    {
        var pointer = Marshal.AllocHGlobal(1);
        Marshal.WriteByte(pointer, 0, 0);
        return pointer;
    }

    private void CreateHandlers(NamePlateUpdateContext createdContext)
    {
        var handlers = new List<NamePlateUpdateHandler>();
        for (var i = 0; i < AddonNamePlate.NumNamePlateObjects; i++)
        {
            handlers.Add(new NamePlateUpdateHandler(createdContext, i));
        }

        this.updateHandlers = handlers.ToArray();
    }

    private unsafe void OnRequestedUpdateDetour(
        AtkUnitBase* addon, NumberArrayData** numberArrayData, StringArrayData** stringArrayData)
    {
        var calledOriginal = false;

        try
        {
            if (this.DataUpdate == null && this.NamePlateUpdate == null && this.PostDataUpdate == null &&
                this.PostNamePlateUpdate == null)
            {
                return;
            }

            if (this.context == null)
            {
                this.context = new NamePlateUpdateContext(this.objectTable);
                this.CreateHandlers(this.context);
            }

            this.context.ResetState(addon);

            var activeNamePlateCount = this.context!.ActiveNamePlateCount;
            if (activeNamePlateCount == 0)
                return;

            var activeHandlers = this.updateHandlers[..activeNamePlateCount];

            if (this.pendingForceRedraw)
            {
                this.pendingForceRedraw = false;
                this.context.IsFullUpdate = true;

                foreach (var handler in activeHandlers)
                {
                    handler.ResetState();
                    handler.IsUpdating = true;
                }

                this.DataUpdate?.InvokeSafely(this.context, activeHandlers);
                this.NamePlateUpdate?.InvokeSafely(this.context, activeHandlers);

                if (this.context.HasParts)
                    this.ApplyBuilders(activeHandlers);

                try
                {
                    calledOriginal = true;
                    this.onRequestedUpdateHook.Original.Invoke(addon, numberArrayData, stringArrayData);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Caught exception when calling original AddonNamePlate OnRequestedUpdate.");
                }

                this.PostNamePlateUpdate?.InvokeSafely(this.context, activeHandlers);
                this.PostDataUpdate?.InvokeSafely(this.context, activeHandlers);
            }
            else
            {
                var updatedHandlers = new List<NamePlateUpdateHandler>(activeNamePlateCount);
                foreach (var handler in activeHandlers)
                {
                    handler.ResetState();
                    if (handler.IsUpdating)
                        updatedHandlers.Add(handler);
                }

                if (this.DataUpdate is not null)
                {
                    this.DataUpdate?.InvokeSafely(this.context, activeHandlers);
                    this.NamePlateUpdate?.InvokeSafely(this.context, updatedHandlers);

                    if (this.context.HasParts)
                        this.ApplyBuilders(activeHandlers);

                    try
                    {
                        calledOriginal = true;
                        this.onRequestedUpdateHook.Original.Invoke(addon, numberArrayData, stringArrayData);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Caught exception when calling original AddonNamePlate OnRequestedUpdate.");
                    }

                    this.PostNamePlateUpdate?.InvokeSafely(this.context, updatedHandlers);
                    this.PostDataUpdate?.InvokeSafely(this.context, activeHandlers);
                }
                else if (updatedHandlers.Count != 0)
                {
                    this.NamePlateUpdate?.InvokeSafely(this.context, updatedHandlers);

                    if (this.context.HasParts)
                        this.ApplyBuilders(updatedHandlers);

                    try
                    {
                        calledOriginal = true;
                        this.onRequestedUpdateHook.Original.Invoke(addon, numberArrayData, stringArrayData);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Caught exception when calling original AddonNamePlate OnRequestedUpdate.");
                    }

                    this.PostNamePlateUpdate?.InvokeSafely(this.context, updatedHandlers);
                    this.PostDataUpdate?.InvokeSafely(this.context, activeHandlers);
                }
            }
        }
        finally
        {
            if (!calledOriginal)
            {
                try
                {
                    this.onRequestedUpdateHook.Original.Invoke(addon, numberArrayData, stringArrayData);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Caught exception when calling original AddonNamePlate OnRequestedUpdate.");
                }
            }
        }
    }

    private void ApplyBuilders(Span<NamePlateUpdateHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            if (handler.PartsContainer is { } container)
            {
                container.ApplyBuilders(handler);
            }
        }
    }

    private void ApplyBuilders(List<NamePlateUpdateHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            if (handler.PartsContainer is { } container)
            {
                container.ApplyBuilders(handler);
            }
        }
    }
}
