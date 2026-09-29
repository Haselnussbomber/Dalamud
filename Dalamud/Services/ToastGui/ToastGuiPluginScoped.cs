using Dalamud.IoC.Internal;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ToastGui;

/// <summary>
/// Plugin scoped version of ToastGui.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IToastGui>]
#pragma warning restore SA1015
internal class ToastGuiPluginScoped : IInternalDisposableService, IToastGui
{
    [ServiceManager.ServiceDependency]
    private readonly ToastGui toastGuiService = Service<ToastGui>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastGuiPluginScoped"/> class.
    /// </summary>
    internal ToastGuiPluginScoped()
    {
        this.toastGuiService.Toast += this.ToastForward;
        this.toastGuiService.QuestToast += this.QuestToastForward;
        this.toastGuiService.ErrorToast += this.ErrorToastForward;
    }

    /// <inheritdoc/>
    public event IToastGui.NormalToastDelegate? Toast;

    /// <inheritdoc/>
    public event IToastGui.QuestToastDelegate? QuestToast;

    /// <inheritdoc/>
    public event IToastGui.ErrorToastDelegate? ErrorToast;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.toastGuiService.Toast -= this.ToastForward;
        this.toastGuiService.QuestToast -= this.QuestToastForward;
        this.toastGuiService.ErrorToast -= this.ErrorToastForward;

        this.Toast = null;
        this.QuestToast = null;
        this.ErrorToast = null;
    }

    /// <inheritdoc/>
    public void ShowNormal(ReadOnlySeString message, ToastOptions? options = null) => this.toastGuiService.ShowNormal(message, options);

    /// <inheritdoc/>
    public void ShowQuest(ReadOnlySeString message, QuestToastOptions? options = null) => this.toastGuiService.ShowQuest(message, options);

    /// <inheritdoc/>
    public void ShowError(ReadOnlySeString message) => this.toastGuiService.ShowError(message);

    private void ToastForward(ref ReadOnlySeString message, ref ToastOptions options, ref bool isHandled)
        => this.Toast?.Invoke(ref message, ref options, ref isHandled);

    private void QuestToastForward(ref ReadOnlySeString message, ref QuestToastOptions options, ref bool isHandled)
        => this.QuestToast?.Invoke(ref message, ref options, ref isHandled);

    private void ErrorToastForward(ref ReadOnlySeString message, ref bool isHandled)
        => this.ErrorToast?.Invoke(ref message, ref isHandled);
}
