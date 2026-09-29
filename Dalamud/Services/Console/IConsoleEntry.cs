namespace Dalamud.Services.Console;

/// <summary>
/// Interface representing an entry in the console.
/// </summary>
public interface IConsoleEntry
{
    /// <summary>
    /// Gets the name of the entry.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the entry.
    /// </summary>
    string Description { get; }
}
