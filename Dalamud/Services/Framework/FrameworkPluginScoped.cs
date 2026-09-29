using System.Threading;
using System.Threading.Tasks;

using Dalamud.IoC.Internal;
using Dalamud.Plugin.Internal;
using Dalamud.Plugin.Internal.Types;
using Dalamud.Utility;

namespace Dalamud.Services.Framework;

/// <summary>
/// Plugin-scoped version of a Framework service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IFramework>]
#pragma warning restore SA1015
internal sealed class FrameworkPluginScoped : IInternalDisposableService, IFramework
{
    private readonly LocalPlugin plugin;
    private readonly PluginErrorHandler pluginErrorHandler;

    [ServiceManager.ServiceDependency]
    private readonly Framework frameworkService = Service<Framework>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPluginScoped"/> class.
    /// </summary>
    /// <param name="plugin">The plugin.</param>
    /// <param name="pluginErrorHandler">Error handler instance.</param>
    internal FrameworkPluginScoped(LocalPlugin plugin, PluginErrorHandler pluginErrorHandler)
    {
        this.plugin = plugin;
        this.pluginErrorHandler = pluginErrorHandler;

        this.frameworkService.Update += this.OnUpdateForward;
    }

    /// <inheritdoc/>
    public event IFramework.UpdateDelegate? Update;

    /// <inheritdoc/>
    public DateTime LastUpdate => this.frameworkService.LastUpdate;

    /// <inheritdoc/>
    public DateTime LastUpdateUTC => this.frameworkService.LastUpdateUTC;

    /// <inheritdoc/>
    public TimeSpan UpdateDelta => this.frameworkService.UpdateDelta;

    /// <inheritdoc/>
    public bool IsInFrameworkUpdateThread => this.frameworkService.IsInFrameworkUpdateThread;

    /// <inheritdoc/>
    public bool IsFrameworkUnloading => this.frameworkService.IsFrameworkUnloading;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.frameworkService.Update -= this.OnUpdateForward;

        this.Update = null;
    }

    /// <inheritdoc/>
    public TaskFactory GetTaskFactory() => this.frameworkService.GetTaskFactory();

    /// <inheritdoc/>
    public Task DelayTicks(long numTicks, CancellationToken cancellationToken = default) =>
        this.frameworkService.DelayTicks(numTicks, cancellationToken);

    /// <inheritdoc/>
    public Task Run(Action action, CancellationToken cancellationToken = default) =>
        this.frameworkService.Run(action, cancellationToken);

    /// <inheritdoc/>
    public Task<T> Run<T>(Func<T> action, CancellationToken cancellationToken = default) =>
        this.frameworkService.Run(action, cancellationToken);

    /// <inheritdoc/>
    public Task Run(Func<Task> action, CancellationToken cancellationToken = default) =>
        this.frameworkService.Run(action, cancellationToken);

    /// <inheritdoc/>
    public Task<T> Run<T>(Func<Task<T>> action, CancellationToken cancellationToken = default) =>
        this.frameworkService.Run(action, cancellationToken);

    /// <inheritdoc/>
    public Task<T> RunOnFrameworkThread<T>(Func<T> func)
        => this.frameworkService.RunOnFrameworkThread(func);

    /// <inheritdoc/>
    public Task RunOnFrameworkThread(Action action)
        => this.frameworkService.RunOnFrameworkThread(action);

    /// <inheritdoc/>
    public Task<T> RunOnFrameworkThread<T>(Func<Task<T>> func)
        => this.frameworkService.RunOnFrameworkThread(func);

    /// <inheritdoc/>
    public Task RunOnFrameworkThread(Func<Task> func)
        => this.frameworkService.RunOnFrameworkThread(func);

    /// <inheritdoc/>
    public Task<T> RunOnTick<T>(Func<T> func, TimeSpan delay = default, int delayTicks = default, CancellationToken cancellationToken = default)
        => this.frameworkService.RunOnTick(func, delay, delayTicks, cancellationToken);

    /// <inheritdoc/>
    public Task RunOnTick(Action action, TimeSpan delay = default, int delayTicks = default, CancellationToken cancellationToken = default)
        => this.frameworkService.RunOnTick(action, delay, delayTicks, cancellationToken);

    /// <inheritdoc/>
    public Task<T> RunOnTick<T>(Func<Task<T>> func, TimeSpan delay = default, int delayTicks = default, CancellationToken cancellationToken = default)
        => this.frameworkService.RunOnTick(func, delay, delayTicks, cancellationToken);

    /// <inheritdoc/>
    public Task RunOnTick(Func<Task> func, TimeSpan delay = default, int delayTicks = default, CancellationToken cancellationToken = default)
        => this.frameworkService.RunOnTick(func, delay, delayTicks, cancellationToken);

    /// <inheritdoc/>
    public IDebouncer CreateDebouncer(TimeSpan delay, Action action)
        => this.frameworkService.CreateDebouncer(delay, action);

    private void OnUpdateForward(IFramework framework)
    {
        this.frameworkService.ProfileAndInvoke(this.Update, this, (ex, handlerName) =>
        {
            Serilog.Log.Error(ex, $"[{this.plugin.InternalName}] Exception in event handler {{EventHandlerName}}", handlerName);
            this.pluginErrorHandler.NotifyError();
        });
    }
}
