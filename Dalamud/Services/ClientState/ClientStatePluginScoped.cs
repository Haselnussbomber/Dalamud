using Dalamud.Common;
using Dalamud.IoC;
using Dalamud.IoC.Internal;
using Dalamud.Services.Conditions;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using Action = System.Action;

namespace Dalamud.Services.ClientState;

/// <summary>
/// Plugin-scoped version of a GameConfig service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IClientState>]
#pragma warning restore SA1015
internal class ClientStatePluginScoped : IInternalDisposableService, IClientState
{
    [ServiceManager.ServiceDependency]
    private readonly ClientState clientStateService = Service<ClientState>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientStatePluginScoped"/> class.
    /// </summary>
    internal ClientStatePluginScoped()
    {
        this.clientStateService.ZoneInit += this.ZoneInitForward;
        this.clientStateService.TerritoryChanged += this.TerritoryChangedForward;
        this.clientStateService.MapChanged += this.MapChangedForward;
        this.clientStateService.InstanceChanged += this.InstanceChangedForward;
        this.clientStateService.ClassJobChanged += this.ClassJobChangedForward;
        this.clientStateService.LevelChanged += this.LevelChangedForward;
        this.clientStateService.Login += this.LoginForward;
        this.clientStateService.Logout += this.LogoutForward;
        this.clientStateService.EnterPvP += this.EnterPvPForward;
        this.clientStateService.LeavePvP += this.ExitPvPForward;
        this.clientStateService.CfPop += this.ContentFinderPopForward;
    }

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
    public ClientLanguage ClientLanguage => this.clientStateService.ClientLanguage;

    /// <inheritdoc/>
    public RowRef<TerritoryType> TerritoryType => this.clientStateService.TerritoryType;

    /// <inheritdoc/>
    public RowRef<Map> Map => this.clientStateService.Map;

    /// <inheritdoc/>
    public uint Instance => this.clientStateService.Instance;

    /// <inheritdoc/>
    public bool IsLoggedIn => this.clientStateService.IsLoggedIn;

    /// <inheritdoc/>
    public bool IsPvP => this.clientStateService.IsPvP;

    /// <inheritdoc/>
    public bool IsPvPExcludingDen => this.clientStateService.IsPvPExcludingDen;

    /// <inheritdoc/>
    public bool IsGPosing => this.clientStateService.IsGPosing;

    /// <inheritdoc/>
    public bool IsClientIdle(out ConditionFlag blockingFlag) => this.clientStateService.IsClientIdle(out blockingFlag);

    /// <inheritdoc/>
    public bool IsClientIdle() => this.clientStateService.IsClientIdle();

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.clientStateService.ZoneInit -= this.ZoneInitForward;
        this.clientStateService.TerritoryChanged -= this.TerritoryChangedForward;
        this.clientStateService.MapChanged -= this.MapChangedForward;
        this.clientStateService.InstanceChanged -= this.InstanceChangedForward;
        this.clientStateService.ClassJobChanged -= this.ClassJobChangedForward;
        this.clientStateService.LevelChanged -= this.LevelChangedForward;
        this.clientStateService.Login -= this.LoginForward;
        this.clientStateService.Logout -= this.LogoutForward;
        this.clientStateService.EnterPvP -= this.EnterPvPForward;
        this.clientStateService.LeavePvP -= this.ExitPvPForward;
        this.clientStateService.CfPop -= this.ContentFinderPopForward;

        this.ZoneInit = null;
        this.TerritoryChanged = null;
        this.MapChanged = null;
        this.InstanceChanged = null;
        this.ClassJobChanged = null;
        this.LevelChanged = null;
        this.Login = null;
        this.Logout = null;
        this.EnterPvP = null;
        this.LeavePvP = null;
        this.CfPop = null;
    }

    private void ZoneInitForward(ZoneInitEventArgs eventArgs) => this.ZoneInit?.Invoke(eventArgs);

    private void TerritoryChangedForward(RowRef<TerritoryType> territoryType) => this.TerritoryChanged?.Invoke(territoryType);

    private void MapChangedForward(RowRef<Map> map) => this.MapChanged?.Invoke(map);

    private void InstanceChangedForward(uint instanceId) => this.InstanceChanged?.Invoke(instanceId);

    private void ClassJobChangedForward(RowRef<ClassJob> classJob) => this.ClassJobChanged?.Invoke(classJob);

    private void LevelChangedForward(RowRef<ClassJob> classJob, uint level) => this.LevelChanged?.Invoke(classJob, level);

    private void LoginForward() => this.Login?.Invoke();

    private void LogoutForward(int type, int code) => this.Logout?.Invoke(type, code);

    private void EnterPvPForward() => this.EnterPvP?.Invoke();

    private void ExitPvPForward() => this.LeavePvP?.Invoke();

    private void ContentFinderPopForward(RowRef<ContentFinderCondition> cfc) => this.CfPop?.Invoke(cfc);
}
