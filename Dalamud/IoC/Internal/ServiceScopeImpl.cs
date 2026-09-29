using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dalamud.Services.Framework;
using Dalamud.Utility;

namespace Dalamud.IoC.Internal;

/// <summary>
/// Implementation of a service scope.
/// </summary>
internal class ServiceScopeImpl : IServiceScope
{
    private readonly ServiceContainer container;

    private readonly List<object> privateScopedObjects = [];
    private readonly ConcurrentDictionary<Type, Task<object>> scopeCreatedObjects = new();

    private readonly ReaderWriterLockSlim disposeLock = new(LockRecursionPolicy.SupportsRecursion);
    private bool disposed;

    /// <summary>Initializes a new instance of the <see cref="ServiceScopeImpl" /> class.</summary>
    /// <param name="container">The container this scope will use to create services.</param>
    public ServiceScopeImpl(ServiceContainer container) => this.container = container;

    /// <inheritdoc/>
    public object? GetService(Type serviceType)
    {
        return this.container.GetService(serviceType, this, []).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void RegisterPrivateScopes(params object[] scopes)
    {
        this.disposeLock.EnterReadLock();
        try
        {
            ObjectDisposedException.ThrowIf(this.disposed, this);
            this.privateScopedObjects.AddRange(scopes);
        }
        finally
        {
            this.disposeLock.ExitReadLock();
        }
    }

    /// <inheritdoc />
    public Task<object> CreateAsync(Type objectType, ObjectInstanceVisibility allowedVisibility, params object[] scopedObjects)
    {
        this.disposeLock.EnterReadLock();
        try
        {
            ObjectDisposedException.ThrowIf(this.disposed, this);
            return this.container.CreateAsync(objectType, allowedVisibility, scopedObjects, this);
        }
        finally
        {
            this.disposeLock.ExitReadLock();
        }
    }

    /// <inheritdoc />
    public Task InjectPropertiesAsync(object instance, params object[] scopedObjects)
    {
        this.disposeLock.EnterReadLock();
        try
        {
            ObjectDisposedException.ThrowIf(this.disposed, this);
            return this.container.InjectProperties(instance, scopedObjects, this);
        }
        finally
        {
            this.disposeLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Create a service scoped to this scope, with private scoped objects.
    /// </summary>
    /// <param name="objectType">The type of object to create.</param>
    /// <param name="scopedObjects">Additional scoped objects.</param>
    /// <returns>The created object, or null.</returns>
    public Task<object> CreatePrivateScopedObject(Type objectType, params object[] scopedObjects)
    {
        this.disposeLock.EnterReadLock();
        try
        {
            ObjectDisposedException.ThrowIf(this.disposed, this);
            return this.scopeCreatedObjects.GetOrAdd(
                objectType,
                static (objectType, p) => p.Scope.container.CreateAsync(
                    objectType,
                    ObjectInstanceVisibility.Internal, // We are allowed to resolve internal services here since this is a private scoped object.
                    p.Objects.Concat(p.Scope.privateScopedObjects).ToArray(),
                    p.Scope),
                (Scope: this, Objects: scopedObjects));
        }
        finally
        {
            this.disposeLock.ExitReadLock();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this.disposeLock.EnterWriteLock();
        this.disposed = true;
        this.disposeLock.ExitWriteLock();

        List<Exception>? exceptions = null;
        while (!this.scopeCreatedObjects.IsEmpty)
        {
            try
            {
                await Task.WhenAll(
                    this.scopeCreatedObjects.Keys.Select(
                        async type =>
                        {
                            if (!this.scopeCreatedObjects.Remove(type, out var serviceTask))
                                return;

                            switch (await serviceTask)
                            {
                                case IInternalDisposableService d:
                                    d.DisposeService();
                                    break;
                                case IAsyncDisposable d:
                                    await d.DisposeAsync();
                                    break;
                                case IDisposable d:
                                    d.Dispose();
                                    break;
                            }
                        }));
            }
            catch (AggregateException ae)
            {
                exceptions ??= [];
                exceptions.AddRange(ae.Flatten().InnerExceptions);
            }
        }

        // Unless Dalamud is unloading (plugin cannot be reloading at that point), ensure that there are no more
        // event callback call in progress when this function returns. Since above service dispose operations should
        // have unregistered the event listeners, on next framework tick, none can be running anymore.
        // This has an additional effect of ensuring that DtrBar entries are completely removed on return.
        // Note that this still does not handle Framework.RunOnTick with specified delays.
        await (Service<Framework>.GetNullable()?.DelayTicks(1) ?? Task.CompletedTask).SuppressException();

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }
}
