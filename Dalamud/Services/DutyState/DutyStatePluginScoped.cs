using Dalamud.IoC.Internal;

using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace Dalamud.Services.DutyState;

/// <summary>
/// Plugin scoped version of DutyState.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IDutyState>]
#pragma warning restore SA1015
internal class DutyStatePluginScoped : IInternalDisposableService, IDutyState
{
    [ServiceManager.ServiceDependency]
    private readonly DutyState dutyStateService = Service<DutyState>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="DutyStatePluginScoped"/> class.
    /// </summary>
    internal DutyStatePluginScoped()
    {
        this.dutyStateService.DutyStarted += this.DutyStartedForward;
        this.dutyStateService.DutyWiped += this.DutyWipedForward;
        this.dutyStateService.DutyRecommenced += this.DutyRecommencedForward;
        this.dutyStateService.DutyCompleted += this.DutyCompletedForward;
    }

    /// <inheritdoc/>
    public event IDutyState.DutyStartedDelegate? DutyStarted;

    /// <inheritdoc/>
    public event IDutyState.DutyWipedDelegate? DutyWiped;

    /// <inheritdoc/>
    public event IDutyState.DutyRecommencedDelegate? DutyRecommenced;

    /// <inheritdoc/>
    public event IDutyState.DutyCompletedDelegate? DutyCompleted;

    /// <inheritdoc/>
    public RowRef<ContentFinderCondition> ContentFinderCondition => this.dutyStateService.ContentFinderCondition;

    /// <inheritdoc/>
    public bool IsDutyStarted => this.dutyStateService.IsDutyStarted;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.dutyStateService.DutyStarted -= this.DutyStartedForward;
        this.dutyStateService.DutyWiped -= this.DutyWipedForward;
        this.dutyStateService.DutyRecommenced -= this.DutyRecommencedForward;
        this.dutyStateService.DutyCompleted -= this.DutyCompletedForward;

        this.DutyStarted = null;
        this.DutyWiped = null;
        this.DutyRecommenced = null;
        this.DutyCompleted = null;
    }

    private void DutyStartedForward(IDutyStateEventArgs args) => this.DutyStarted?.Invoke(args);

    private void DutyWipedForward(IDutyStateEventArgs args) => this.DutyWiped?.Invoke(args);

    private void DutyRecommencedForward(IDutyStateEventArgs args) => this.DutyRecommenced?.Invoke(args);

    private void DutyCompletedForward(IDutyStateEventArgs args) => this.DutyCompleted?.Invoke(args);
}
