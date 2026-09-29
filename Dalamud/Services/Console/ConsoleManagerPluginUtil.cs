using System.Text.RegularExpressions;

namespace Dalamud.Services.Console;

/// <summary>
/// Utility functions for the console manager.
/// </summary>
internal static partial class ConsoleManagerPluginUtil
{
    private static readonly string[] ReservedNamespaces = ["dalamud", "xl", "plugin"];

    /// <summary>
    /// Get a sanitized namespace name from a plugin's internal name.
    /// </summary>
    /// <param name="pluginInternalName">The plugin's internal name.</param>
    /// <returns>A sanitized namespace.</returns>
    public static string GetSanitizedNamespaceName(string pluginInternalName)
    {
        // Must be lowercase
        pluginInternalName = pluginInternalName.ToLowerInvariant();

        // Remove all non-alphabetic characters
        pluginInternalName = NonAlphaRegex().Replace(pluginInternalName, string.Empty);

        // Remove reserved namespaces from the start or end
        foreach (var reservedNamespace in ReservedNamespaces)
        {
            if (pluginInternalName.StartsWith(reservedNamespace))
            {
                pluginInternalName = pluginInternalName[reservedNamespace.Length..];
            }

            if (pluginInternalName.EndsWith(reservedNamespace))
            {
                pluginInternalName = pluginInternalName[..^reservedNamespace.Length];
            }
        }

        return pluginInternalName;
    }

    [GeneratedRegex(@"[^a-z]")]
    private static partial Regex NonAlphaRegex();
}
