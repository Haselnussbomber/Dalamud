using Dalamud.IoC.Internal;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.FlyTextGui;

/// <summary>
/// Plugin scoped version of FlyTextGui.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IFlyTextGui>]
#pragma warning restore SA1015
internal class FlyTextGuiPluginScoped : IInternalDisposableService, IFlyTextGui
{
    [ServiceManager.ServiceDependency]
    private readonly FlyTextGui flyTextGuiService = Service<FlyTextGui>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyTextGuiPluginScoped"/> class.
    /// </summary>
    internal FlyTextGuiPluginScoped()
    {
        this.flyTextGuiService.FlyTextCreated += this.FlyTextCreatedForward;
    }

    /// <inheritdoc/>
    public event IFlyTextGui.FlyTextCreatedDelegate? FlyTextCreated;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.flyTextGuiService.FlyTextCreated -= this.FlyTextCreatedForward;

        this.FlyTextCreated = null;
    }

    /// <inheritdoc/>
    public void AddFlyText(FlyTextKind kind, uint actorIndex, uint val1, uint val2, ReadOnlySeString text1, ReadOnlySeString text2, uint color, uint icon, uint damageTypeIcon)
    {
        this.flyTextGuiService.AddFlyText(kind, actorIndex, val1, val2, text1, text2, color, icon, damageTypeIcon);
    }

    private void FlyTextCreatedForward(ref FlyTextKind kind, ref int val1, ref int val2, ref ReadOnlySeString text1, ref ReadOnlySeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
        => this.FlyTextCreated?.Invoke(ref kind, ref val1, ref val2, ref text1, ref text2, ref color, ref icon, ref damageTypeIcon, ref yOffset, ref handled);
}
