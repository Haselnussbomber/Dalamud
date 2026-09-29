using System.Collections.Generic;

using Dalamud.Interface.Textures;
using Dalamud.IoC;
using Dalamud.IoC.Internal;

namespace Dalamud.Services.TitleScreenMenu;

/// <summary>
/// Plugin-scoped version of a TitleScreenMenu service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<ITitleScreenMenu>]
#pragma warning restore SA1015
internal class TitleScreenMenuPluginScoped : IInternalDisposableService, ITitleScreenMenu
{
    [ServiceManager.ServiceDependency]
    private readonly TitleScreenMenu titleScreenMenuService = Service<TitleScreenMenu>.Get();

    private readonly List<IReadOnlyTitleScreenMenuEntry> pluginEntries = [];

    /// <inheritdoc/>
    public IReadOnlyList<IReadOnlyTitleScreenMenuEntry>? Entries => this.titleScreenMenuService.Entries;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        foreach (var entry in this.pluginEntries)
        {
            this.titleScreenMenuService.RemoveEntry(entry);
        }
    }

    /// <inheritdoc/>
    public IReadOnlyTitleScreenMenuEntry AddEntry(string text, ISharedImmediateTexture texture, Action onTriggered)
    {
        var entry = this.titleScreenMenuService.AddPluginEntry(text, texture, onTriggered);
        this.pluginEntries.Add(entry);

        return entry;
    }

    /// <inheritdoc/>
    public IReadOnlyTitleScreenMenuEntry AddEntry(ulong priority, string text, ISharedImmediateTexture texture, Action onTriggered)
    {
        var entry = this.titleScreenMenuService.AddPluginEntry(priority, text, texture, onTriggered);
        this.pluginEntries.Add(entry);

        return entry;
    }

    /// <inheritdoc/>
    public void RemoveEntry(IReadOnlyTitleScreenMenuEntry entry)
    {
        this.pluginEntries.Remove(entry);
        this.titleScreenMenuService.RemoveEntry(entry);
    }
}
