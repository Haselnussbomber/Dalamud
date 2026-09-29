using System.Collections.Generic;

using Dalamud.IoC;
using Dalamud.IoC.Internal;
using Dalamud.Plugin.Internal.Types;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.DtrBar;

/// <summary>
/// Plugin-scoped version of a AddonEventManager service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IDtrBar>]
#pragma warning restore SA1015
internal sealed class DtrBarPluginScoped : IInternalDisposableService, IDtrBar
{
    private readonly LocalPlugin plugin;

    [ServiceManager.ServiceDependency]
    private readonly DtrBar dtrBarService = Service<DtrBar>.Get();

    [ServiceManager.ServiceConstructor]
    private DtrBarPluginScoped(LocalPlugin plugin) => this.plugin = plugin;

    /// <inheritdoc/>
    public IReadOnlyList<IReadOnlyDtrBarEntry> Entries => this.dtrBarService.Entries;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService() => this.dtrBarService.Remove(this.plugin, null);

    /// <inheritdoc/>
    public IDtrBarEntry Get(string title, ReadOnlySeString text = default) => this.dtrBarService.Get(this.plugin, title, text);

    /// <inheritdoc/>
    public void Remove(string title) => this.dtrBarService.Remove(this.plugin, title);
}
