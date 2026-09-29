using System.Collections.Generic;

namespace Dalamud.Services.Console;

/// <summary>
/// Interface representing a command in the console.
/// </summary>
public interface IConsoleCommand : IConsoleEntry
{
    /// <summary>
    /// Execute this command.
    /// </summary>
    /// <param name="arguments">Arguments to invoke the entry with.</param>
    /// <returns>Whether execution succeeded.</returns>
    bool Invoke(IEnumerable<object> arguments);
}
