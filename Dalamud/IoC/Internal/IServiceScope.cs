using System.Threading.Tasks;

namespace Dalamud.IoC.Internal;

/// <summary>
/// Container enabling the creation of scoped services.
/// </summary>
internal interface IServiceScope : IServiceProvider, IAsyncDisposable
{
    /// <summary>
    /// Register objects that may be injected to scoped services,
    /// but not directly to created objects.
    /// </summary>
    /// <param name="scopes">The scopes to add.</param>
    void RegisterPrivateScopes(params object[] scopes);

    /// <summary>
    /// Create an object.
    /// </summary>
    /// <param name="objectType">The type of object to create.</param>
    /// <param name="allowedVisibility">Defines which services are allowed to be directly resolved into this type.</param>
    /// <param name="scopedObjects">Scoped objects to be included in the constructor.</param>
    /// <returns>The created object.</returns>
    Task<object> CreateAsync(Type objectType, ObjectInstanceVisibility allowedVisibility, params object[] scopedObjects);

    /// <summary>
    /// Inject <see cref="PluginInterfaceAttribute" /> interfaces into public or static properties on the provided object.
    /// The properties have to be marked with the <see cref="PluginServiceAttribute" />.
    /// </summary>
    /// <param name="instance">The object instance.</param>
    /// <param name="scopedObjects">Scoped objects to be injected.</param>
    /// <returns>A <see cref="ValueTask"/> representing the status of the operation.</returns>
    Task InjectPropertiesAsync(object instance, params object[] scopedObjects);
}
