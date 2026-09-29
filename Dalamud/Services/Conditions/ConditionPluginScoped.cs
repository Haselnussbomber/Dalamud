using System.Collections.Generic;

using Dalamud.IoC.Internal;

namespace Dalamud.Services.Conditions;

/// <summary>
/// Plugin-scoped version of a Condition service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<ICondition>]
#pragma warning restore SA1015
internal class ConditionPluginScoped : IInternalDisposableService, ICondition
{
    [ServiceManager.ServiceDependency]
    private readonly Condition conditionService = Service<Condition>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionPluginScoped"/> class.
    /// </summary>
    internal ConditionPluginScoped()
    {
        this.conditionService.ConditionChange += this.ConditionChangedForward;
    }

    /// <inheritdoc/>
    public event ICondition.ConditionChangeDelegate? ConditionChange;

    /// <inheritdoc/>
    public int MaxEntries => this.conditionService.MaxEntries;

    /// <inheritdoc/>
    public IntPtr Address => this.conditionService.Address;

    /// <inheritdoc/>
    public bool this[int flag] => this.conditionService[flag];

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.conditionService.ConditionChange -= this.ConditionChangedForward;

        this.ConditionChange = null;
    }

    /// <inheritdoc/>
    public IReadOnlySet<ConditionFlag> AsReadOnlySet() => this.conditionService.AsReadOnlySet();

    /// <inheritdoc/>
    public bool Any() => this.conditionService.Any();

    /// <inheritdoc/>
    public bool Any(params ConditionFlag[] flags) => this.conditionService.Any(flags);

    /// <inheritdoc/>
    public bool AnyExcept(params ConditionFlag[] except) => this.conditionService.AnyExcept(except);

    /// <inheritdoc/>
    public bool OnlyAny(params ConditionFlag[] other) => this.conditionService.OnlyAny(other);

    /// <inheritdoc/>
    public bool EqualTo(params ConditionFlag[] other) => this.conditionService.EqualTo(other);

    private void ConditionChangedForward(ConditionFlag flag, bool value) => this.ConditionChange?.Invoke(flag, value);
}
