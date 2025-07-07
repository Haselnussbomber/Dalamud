using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Dalamud.Data;
using Dalamud.Hooking;
using Dalamud.Logging.Internal;
using Dalamud.Plugin.Services;

using FFXIVClientStructs.FFXIV.Client.UI.Misc;

using Lumina.Excel;

namespace Dalamud.Game.PlayerState;

/// <summary>
/// This class represents the state of the local player.
/// </summary>
internal unsafe partial class PlayerState : IInternalDisposableService, IPlayerState
{
    private static readonly ModuleLog Log = new("PlayerState");

    private readonly PlayerStateAddressResolver address;
    private readonly Hook<PerformMateriaActionMigrationDelegate> performMateriaActionMigrationDelegateHook;
    private readonly ConcurrentDictionary<Type, HashSet<uint>> cachedUnlockedRowIds = [];

    [ServiceManager.ServiceDependency]
    private readonly DataManager dataManager = Service<DataManager>.Get();

    [ServiceManager.ServiceDependency]
    private readonly ClientState.ClientState clientState = Service<ClientState.ClientState>.Get();

    [ServiceManager.ServiceConstructor]
    private PlayerState(TargetSigScanner sigScanner)
    {
        this.address = new PlayerStateAddressResolver();
        this.address.Setup(sigScanner);

        this.clientState.Login += this.OnLogin;
        this.clientState.Logout += this.OnLogout;

        this.performMateriaActionMigrationDelegateHook = Hook<PerformMateriaActionMigrationDelegate>.FromAddress(
            this.address.PerformMateriaActionMigration,
            this.PerformMateriaActionMigrationDetour);

        this.performMateriaActionMigrationDelegateHook.Enable();
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void PerformMateriaActionMigrationDelegate(RaptureHotbarModule* thisPtr);

    /// <inheritdoc/>
    public event EventHandler<RowRef>? Unlock;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.clientState.Login -= this.OnLogin;
        this.clientState.Logout -= this.OnLogout;
        this.performMateriaActionMigrationDelegateHook.Dispose();
    }

    private void OnLogin()
    {
        try
        {
            this.UpdateUnlocks(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during initial unlock check");
        }
    }

    private void OnLogout(int type, int code)
    {
        this.cachedUnlockedRowIds.Clear();
    }
}
