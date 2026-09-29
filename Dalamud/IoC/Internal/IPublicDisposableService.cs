namespace Dalamud.IoC.Internal;

/// <summary>An <see cref="IInternalDisposableService"/> which happens to be public and needs to expose
/// <see cref="IDisposable.Dispose"/>.</summary>
internal interface IPublicDisposableService : IInternalDisposableService, IDisposable
{
    /// <summary>Marks that only <see cref="IInternalDisposableService.DisposeService"/> should respond,
    /// while suppressing <see cref="IDisposable.Dispose"/>.</summary>
    void MarkDisposeOnlyFromService();
}
