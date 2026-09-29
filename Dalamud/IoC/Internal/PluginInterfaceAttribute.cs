namespace Dalamud.IoC.Internal;

/// <summary>
/// This attribute indicates whether the decorated class should be exposed to plugins via IoC.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class PluginInterfaceAttribute : Attribute
{
}
