using System.Collections.Generic;
using System.Reflection;

namespace Dalamud.IoC.Internal;

/// <summary>
/// Helper functions for services.
/// </summary>
internal static class ServiceHelpers
{
    /// <summary>
    /// Get a list of dependencies for a service. Only accepts <see cref="Service{T}"/> types.
    /// These are NOT returned as <see cref="Service{T}"/> types; raw types will be returned.
    /// </summary>
    /// <param name="serviceType">The dependencies for this service.</param>
    /// <param name="includeUnloadDependencies">Whether to include the unload dependencies.</param>
    /// <returns>A list of dependencies.</returns>
    public static IReadOnlyCollection<Type> GetDependencies(Type serviceType, bool includeUnloadDependencies)
    {
#if DEBUG
        if (!serviceType.IsGenericType || serviceType.GetGenericTypeDefinition() != typeof(Service<>))
        {
            throw new ArgumentException(
                $"Expected an instance of {nameof(Service<IServiceType>)}<>",
                nameof(serviceType));
        }
#endif

        return (IReadOnlyCollection<Type>)serviceType.InvokeMember(
                   nameof(Service<IServiceType>.GetDependencyServices),
                   BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                   null,
                   null,
                   [includeUnloadDependencies]) ?? new List<Type>();
    }

    /// <summary>
    /// Get the <see cref="Service{T}"/> type for a given service type.
    /// This will throw if the service type is not a valid service.
    /// </summary>
    /// <param name="type">The type to obtain a <see cref="Service{T}"/> for.</param>
    /// <returns>The <see cref="Service{T}"/>.</returns>
    public static Type GetAsService(Type type)
    {
#if DEBUG
        if (!type.IsAssignableTo(typeof(IServiceType)))
            throw new ArgumentException($"Expected an instance of {nameof(IServiceType)}", nameof(type));
#endif

        return typeof(Service<>).MakeGenericType(type);
    }
}
