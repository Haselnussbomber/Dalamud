namespace Dalamud.IoC.Internal;

/// <summary><see cref="IDisposable"/>, but for <see cref="IServiceType"/>.</summary>
/// <remarks>Use this to prevent services from accidentally being disposed by plugins or <c>using</c> clauses.</remarks>
internal interface IInternalDisposableService : IServiceType
{
    /// <summary>Disposes the service.</summary>
    void DisposeService();
}
