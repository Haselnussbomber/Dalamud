using System.Linq;
using System.Threading.Tasks;

using Dalamud.Common;
using Dalamud.Configuration.Internal;
using Dalamud.Hooking;
using Dalamud.IoC.Internal;
using Dalamud.Logging.Internal;
using Dalamud.Services.Conditions;
using Dalamud.Services.DataManager;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Application.Network;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Network;
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using Action = System.Action;
using CSUIState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState;

namespace Dalamud.Services.ClientState;

/// <summary>
/// This class represents the state of the game client at the time of access.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal sealed unsafe class ClientState : IInternalDisposableService, IClientState
{
    private static readonly ModuleLog Log = ModuleLog.Create<ClientState>();

    [ServiceManager.ServiceDependency]
    private readonly GameLifecycle.GameLifecycle gameLifecycle = Service<GameLifecycle.GameLifecycle>.Get();

    [ServiceManager.ServiceDependency]
    private readonly DalamudConfiguration configuration = Service<DalamudConfiguration>.Get();

    [ServiceManager.ServiceDependency]
    private readonly Framework.Framework framework = Service<Framework.Framework>.Get();

    [ServiceManager.ServiceDependency]
    private readonly ObjectTable.ObjectTable objectTable = Service<ObjectTable.ObjectTable>.Get();

    [ServiceManager.ServiceDependency]
    private readonly ChatGui.ChatGui chatGui = Service<ChatGui.ChatGui>.Get();

    private readonly Hook<UIModule.Delegates.HandlePacket> uiModuleHandlePacketHook;
    private readonly Hook<PacketDispatcher.Delegates.HandleContentsFinderNotificationPacket> cfPopHook;

    private Hook<LogoutCallbackInterface.Delegates.OnLogout>? onLogoutHook;
    private bool initialized;
    private bool lastConditionNone = true;

    [ServiceManager.ServiceConstructor]
    private ClientState(Dalamud dalamud)
    {
        Log.Verbose("===== C L I E N T  S T A T E =====");

        this.ClientLanguage = (ClientLanguage)dalamud.StartInfo.Language;

        this.uiModuleHandlePacketHook = Hook<UIModule.Delegates.HandlePacket>.FromAddress(
            (nint)UIModule.StaticVirtualTablePointer->HandlePacket,
            this.UIModuleHandlePacketDetour);

        this.cfPopHook = Hook<PacketDispatcher.Delegates.HandleContentsFinderNotificationPacket>.FromAddress(
            PacketDispatcher.Addresses.HandleContentsFinderNotificationPacket.Value,
            this.HandleContentsFinderNotificationPacketDetour);

        this.uiModuleHandlePacketHook.Enable();
        this.cfPopHook.Enable();

        this.framework.RunOnTick(this.Setup);
    }

    private delegate void SetCurrentInstanceDelegate(NetworkModuleProxy* thisPtr, short instanceId);

    /// <inheritdoc/>
    public event Action<ZoneInitEventArgs>? ZoneInit;

    /// <inheritdoc/>
    public event Action<RowRef<TerritoryType>>? TerritoryChanged;

    /// <inheritdoc/>
    public event Action<RowRef<Map>>? MapChanged;

    /// <inheritdoc/>
    public event Action<uint>? InstanceChanged;

    /// <inheritdoc/>
    public event IClientState.ClassJobChangeDelegate? ClassJobChanged;

    /// <inheritdoc/>
    public event IClientState.LevelChangeDelegate? LevelChanged;

    /// <inheritdoc/>
    public event Action? Login;

    /// <inheritdoc/>
    public event IClientState.LogoutDelegate? Logout;

    /// <inheritdoc/>
    public event Action? EnterPvP;

    /// <inheritdoc/>
    public event Action? LeavePvP;

    /// <inheritdoc/>
    public event Action<RowRef<ContentFinderCondition>>? CfPop;

    /// <inheritdoc/>
    public ClientLanguage ClientLanguage { get; }

    /// <inheritdoc/>
    public RowRef<TerritoryType> TerritoryType
    {
        get;
        private set
        {
            if (field.RowId != value.RowId)
            {
                field = value;

                if (this.initialized)
                {
                    Log.Debug("TerritoryType changed: {0}", value.RowId);
                    this.TerritoryChanged?.InvokeSafely(value);
                }
            }
        }
    }

    /// <inheritdoc/>
    public RowRef<Map> Map
    {
        get;
        private set
        {
            if (field.RowId != value.RowId)
            {
                field = value;

                if (this.initialized)
                {
                    Log.Debug("MapId changed: {0}", value.RowId);
                    this.MapChanged?.InvokeSafely(value);
                }
            }
        }
    }

    /// <inheritdoc/>
    public uint Instance
    {
        get;
        private set
        {
            if (field != value)
            {
                field = value;

                if (this.initialized)
                {
                    Log.Debug("Instance changed: {0}", value);
                    this.InstanceChanged?.InvokeSafely(value);
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool IsLoggedIn
    {
        get
        {
            var agentLobby = AgentLobby.Instance();
            return agentLobby != null && agentLobby->IsLoggedIn;
        }
    }

    /// <inheritdoc/>
    public bool IsPvP
    {
        get;
        private set
        {
            if (field != value)
            {
                field = value;

                if (this.initialized)
                {
                    if (value)
                    {
                        Log.Debug("EnterPvP");
                        this.EnterPvP?.InvokeSafely();
                    }
                    else
                    {
                        Log.Debug("LeavePvP");
                        this.LeavePvP?.InvokeSafely();
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool IsPvPExcludingDen => this.IsPvP && this.TerritoryType.RowId != 250;

    /// <inheritdoc />
    public bool IsGPosing => GameMain.IsInGPose();

    /// <inheritdoc/>
    public bool IsClientIdle(out ConditionFlag blockingFlag)
    {
        blockingFlag = 0;
        if (!this.IsLoggedIn)
            return true;

        var condition = Service<Conditions.Condition>.GetNullable();

        var blockingConditions = condition.AsReadOnlySet().Except([
            ConditionFlag.NormalConditions,
            ConditionFlag.Emoting,
            ConditionFlag.Jumping,
            ConditionFlag.Mounted,
            ConditionFlag.InFlight,
            ConditionFlag.Swimming,
            ConditionFlag.Diving,
            ConditionFlag.UsingFashionAccessory,
            ConditionFlag.OnFreeTrial]);

        blockingFlag = blockingConditions.FirstOrDefault();
        return blockingFlag == 0;
    }

    /// <inheritdoc/>
    public bool IsClientIdle() => this.IsClientIdle(out _);

    /// <summary>
    /// Dispose of managed and unmanaged resources.
    /// </summary>
    void IInternalDisposableService.DisposeService()
    {
        this.uiModuleHandlePacketHook.Dispose();
        this.cfPopHook.Dispose();
        this.onLogoutHook?.Dispose();

        this.framework.Update -= this.OnFrameworkUpdate;
    }

    private void Setup()
    {
        this.onLogoutHook = Hook<LogoutCallbackInterface.Delegates.OnLogout>.FromAddress((nint)AgentLobby.Instance()->LogoutCallbackInterface.VirtualTable->OnLogout, this.OnLogoutDetour);
        this.onLogoutHook.Enable();

        this.IsPvP = GameMain.IsInPvPArea();
        this.TerritoryType = LuminaUtils.CreateRef<TerritoryType>(GameMain.Instance()->CurrentTerritoryTypeId);
        this.Map = LuminaUtils.CreateRef<Map>(AgentMap.Instance()->CurrentMapId);
        this.Instance = CSUIState.Instance()->PublicInstance.InstanceId;

        this.initialized = true;

        this.framework.Update += this.OnFrameworkUpdate;
    }

    private void UIModuleHandlePacketDetour(
        UIModule* thisPtr, UIModulePacketType type, uint uintParam, void* packet)
    {
        this.uiModuleHandlePacketHook.Original(thisPtr, type, uintParam, packet);

        switch (type)
        {
            case UIModulePacketType.ClassJobChange:
            {
                var classJob = LuminaUtils.CreateRef<ClassJob>(uintParam);

                foreach (var action in Delegate.EnumerateInvocationList(this.ClassJobChanged))
                {
                    try
                    {
                        action(classJob);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Exception during raise of {handler}", action.Method);
                    }
                }

                break;
            }

            case UIModulePacketType.LevelChange:
            {
                var classJob = LuminaUtils.CreateRef<ClassJob>(*(uint*)packet);
                var level = *(ushort*)((nint)packet + 4);

                foreach (var action in Delegate.EnumerateInvocationList(this.LevelChanged))
                {
                    try
                    {
                        action(classJob, level);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Exception during raise of {handler}", action.Method);
                    }
                }

                break;
            }

            case UIModulePacketType.ZoneInit:
            {
                var eventArgs = ZoneInitEventArgs.Read((ZoneInitPacket*)packet);
                Log.Debug($"ZoneInit: {eventArgs}");
                this.ZoneInit?.InvokeSafely(eventArgs);
                this.TerritoryType = eventArgs.TerritoryType;
                this.Instance = eventArgs.Instance;
                this.IsPvP = eventArgs.TerritoryType.Value.IsPvpZone;
                break;
            }
        }
    }

    private void HandleContentsFinderNotificationPacketDetour(ContentsFinderNotificationPacket* packet)
    {
        this.cfPopHook.OriginalDisposeSafe(packet);

        try
        {
            if (packet->QueueState != ContentsFinderQueueState.Ready)
                return;

            if (this.configuration.DutyFinderTaskbarFlash)
                Util.FlashWindow();

            var cfcId = packet->ContentFinderConditionId;
            var cfCondition = LuminaUtils.CreateRef<ContentFinderCondition>(cfcId);

            if (!cfCondition.IsValid)
            {
                Log.Error("CFC key {cfcId} not found", cfcId);
                return;
            }

            var cfcName = cfCondition.Value.Name;
            if (cfcName.IsEmpty)
                cfcName = "Duty Roulette";

            Task.Run(() =>
            {
                if (this.configuration.DutyFinderChatMessage)
                {
                    using var rssb = new RentedSeStringBuilder();
                    this.chatGui.Print(rssb.Builder
                        .Append("Duty pop: ")
                        .Append(cfcName)
                        .ToReadOnlySeString());
                }

                this.CfPop.InvokeSafely(cfCondition);
            }).ContinueWith(
                task => Log.Error(task.Exception, "CfPop.Invoke failed"),
                TaskContinuationOptions.OnlyOnFaulted);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "CfPopDetour threw an exception");
        }
    }

    private void OnFrameworkUpdate(IFramework frameworkArg)
    {
        this.Map = LuminaUtils.CreateRef<Map>(AgentMap.Instance()->CurrentMapId);

        var condition = Service<Conditions.Condition>.GetNullable();
        var gameGui = Service<GameGui.GameGui>.GetNullable();
        var data = Service<DataManager.DataManager>.GetNullable();

        if (condition == null || gameGui == null || data == null)
            return;

        if (condition.Any() && this.lastConditionNone && this.objectTable.LocalPlayer != null)
        {
            Log.Debug("Is login");
            this.lastConditionNone = false;
            this.Login?.InvokeSafely();
            gameGui.ResetUiHideState();

            this.gameLifecycle.ResetLogout();
        }
    }

    private void OnLogoutDetour(LogoutCallbackInterface* thisPtr, LogoutCallbackInterface.LogoutParams* logoutParams)
    {
        var gameGui = Service<GameGui.GameGui>.GetNullable();

        if (logoutParams != null)
        {
            try
            {
                var type = logoutParams->Type;
                var code = logoutParams->Code;

                Log.Debug("Logout: Type {type}, Code {code}", type, code);

                foreach (var action in Delegate.EnumerateInvocationList(this.Logout))
                {
                    try
                    {
                        action(type, code);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Exception during raise of {handler}", action.Method);
                    }
                }

                gameGui?.ResetUiHideState();
                this.lastConditionNone = true; // unblock login flag

                this.gameLifecycle.SetLogout();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception during OnLogoutDetour");
            }
        }

        this.onLogoutHook!.Original(thisPtr, logoutParams);
    }

    private void NetworkHandlersOnCfPop(RowRef<ContentFinderCondition> cfc)
    {
        this.CfPop?.InvokeSafely(cfc);
    }
}
