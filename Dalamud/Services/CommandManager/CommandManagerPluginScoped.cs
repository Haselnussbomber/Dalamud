using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

using Dalamud.IoC;
using Dalamud.IoC.Internal;
using Dalamud.Logging.Internal;
using Dalamud.Plugin.Internal.Types;

namespace Dalamud.Services.CommandManager;

/// <summary>
/// Plugin-scoped version of a AddonLifecycle service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<ICommandManager>]
#pragma warning restore SA1015
internal class CommandManagerPluginScoped : IInternalDisposableService, ICommandManager
{
    private static readonly ModuleLog Log = ModuleLog.Create<CommandManager>();

    [ServiceManager.ServiceDependency]
    private readonly CommandManager commandManagerService = Service<CommandManager>.Get();

    private readonly List<string> pluginRegisteredCommands = [];
    private readonly LocalPlugin pluginInfo;

    private readonly Lock commandListLock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandManagerPluginScoped"/> class.
    /// </summary>
    /// <param name="localPlugin">Info for the plugin that requests this service.</param>
    public CommandManagerPluginScoped(LocalPlugin localPlugin)
    {
        this.pluginInfo = localPlugin;
    }

    /// <inheritdoc/>
    public ReadOnlyDictionary<string, IReadOnlyCommandInfo> Commands => this.commandManagerService.Commands;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        using var scope = this.commandListLock.EnterScope();

        foreach (var command in this.pluginRegisteredCommands)
        {
            this.commandManagerService.RemoveHandler(command);
        }

        this.pluginRegisteredCommands.Clear();
    }

    /// <inheritdoc/>
    public bool ProcessCommand(string content)
        => this.commandManagerService.ProcessCommand(content);

    /// <inheritdoc/>
    public void DispatchCommand(string command, string argument, IReadOnlyCommandInfo info)
        => this.commandManagerService.DispatchCommand(command, argument, info);

    /// <inheritdoc/>
    public bool AddHandler(string command, CommandInfo info)
    {
        using var scope = this.commandListLock.EnterScope();

        if (!this.pluginRegisteredCommands.Contains(command))
        {
            if (this.commandManagerService.AddHandler(command, info, this.pluginInfo.InternalName))
            {
                this.pluginRegisteredCommands.Add(command);
                return true;
            }
        }
        else
        {
            Log.Error("Command {Command} is already registered.", command);
        }

        return false;
    }

    /// <inheritdoc/>
    public bool RemoveHandler(string command)
    {
        using var scope = this.commandListLock.EnterScope();

        if (this.pluginRegisteredCommands.Contains(command))
        {
            if (this.commandManagerService.RemoveHandler(command))
            {
                this.pluginRegisteredCommands.Remove(command);
                return true;
            }
        }
        else
        {
            Log.Error("Command {Command} not found.", command);
        }

        return false;
    }
}
