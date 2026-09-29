using Dalamud.Game.Enums;
using Dalamud.Game.NativeWrapper;
using Dalamud.IoC;
using Dalamud.IoC.Internal;
using Dalamud.Utility;

using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace Dalamud.Services.GameGui;

/// <summary>
/// Plugin-scoped version of a AddonLifecycle service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IGameGui>]
#pragma warning restore SA1015
internal sealed class GameGuiPluginScoped : IInternalDisposableService, IGameGui
{
    [ServiceManager.ServiceDependency]
    private readonly GameGui gameGuiService = Service<GameGui>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameGuiPluginScoped"/> class.
    /// </summary>
    internal GameGuiPluginScoped()
    {
        this.gameGuiService.UiHideToggled += this.UiHideToggledForward;
        this.gameGuiService.HoveredItemChanged += this.HoveredItemForward;
        this.gameGuiService.HoveredActionChanged += this.HoveredActionForward;
        this.gameGuiService.AgentUpdate += this.AgentUpdateForward;
    }

    /// <inheritdoc/>
    public event Action<bool>? UiHideToggled;

    /// <inheritdoc/>
    public event Action<uint>? HoveredItemChanged;

    /// <inheritdoc/>
    public event Action<IHoveredAction>? HoveredActionChanged;

    /// <inheritdoc/>
    public event Action<AgentUpdateFlag> AgentUpdate;

    /// <inheritdoc/>
    public bool GameUiHidden => this.gameGuiService.GameUiHidden;

    /// <inheritdoc/>
    public uint HoveredItem
    {
        get => this.gameGuiService.HoveredItem;
        set => this.gameGuiService.HoveredItem = value;
    }

    /// <inheritdoc/>
    public IHoveredAction HoveredAction => this.gameGuiService.HoveredAction;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.gameGuiService.UiHideToggled -= this.UiHideToggledForward;
        this.gameGuiService.HoveredItemChanged -= this.HoveredItemForward;
        this.gameGuiService.HoveredActionChanged -= this.HoveredActionForward;
        this.gameGuiService.AgentUpdate -= this.AgentUpdateForward;

        this.UiHideToggled = null;
        this.HoveredItemChanged = null;
        this.HoveredActionChanged = null;
    }

    /// <inheritdoc/>
    public bool OpenMapWithMapLink(uint territory, uint map, Vector3 worldPos)
        => this.gameGuiService.OpenMapWithMapLink(territory, map, worldPos);

    /// <inheritdoc/>
    public bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos)
        => this.gameGuiService.WorldToScreen(worldPos, out screenPos);

    /// <inheritdoc/>
    public bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos, out bool inView)
        => this.gameGuiService.WorldToScreen(worldPos, out screenPos, out inView);

    /// <inheritdoc/>
    public bool ScreenToWorld(Vector2 screenPos, out Vector3 worldPos, float rayDistance = 100000)
        => this.gameGuiService.ScreenToWorld(screenPos, out worldPos, rayDistance);

    /// <inheritdoc/>
    public UIModulePtr GetUIModule()
        => this.gameGuiService.GetUIModule();

    /// <inheritdoc/>
    public AtkUnitBasePtr GetAddonByName(string name, int index = 1)
        => this.gameGuiService.GetAddonByName(name, index);

    /// <inheritdoc/>
    public AtkUnitBasePtr GetAddonByName(ReadOnlySpan<byte> name, int index = 1)
        => this.gameGuiService.GetAddonByName(name, index);

    /// <inheritdoc/>
    public unsafe T* GetAddonByName<T>(string name, int index = 1) where T : unmanaged
        => this.gameGuiService.GetAddonByName<T>(name, index);

    /// <inheritdoc/>
    public unsafe T* GetAddonByName<T>(ReadOnlySpan<byte> name, int index = 1) where T : unmanaged
        => this.gameGuiService.GetAddonByName<T>(name, index);

    /// <inheritdoc/>
    public AgentInterfacePtr GetAgentById(int id)
        => this.gameGuiService.GetAgentById(id);

    /// <inheritdoc/>
    public AgentInterfacePtr FindAgentInterface(string addonName)
        => this.gameGuiService.FindAgentInterface(addonName);

    /// <inheritdoc/>
    public AgentInterfacePtr FindAgentInterface(ReadOnlySpan<byte> addonName)
        => this.gameGuiService.FindAgentInterface(addonName);

    /// <inheritdoc/>
    public AgentInterfacePtr FindAgentInterface(AtkUnitBasePtr addon)
        => this.gameGuiService.FindAgentInterface(addon);

    private void UiHideToggledForward(bool toggled) => this.UiHideToggled?.Invoke(toggled);

    private void HoveredItemForward(uint itemId) => this.HoveredItemChanged?.Invoke(itemId);

    private void HoveredActionForward(IHoveredAction hoverAction) => this.HoveredActionChanged?.Invoke(hoverAction);

    private void AgentUpdateForward(AgentUpdateFlag agentUpdateFlag) => this.AgentUpdate.InvokeSafely(agentUpdateFlag);
}
