namespace Dalamud.Services.CommandManager;

/// <summary>
/// Interface representing a registered command.
/// </summary>
public interface IReadOnlyCommandInfo
{
    /// <summary>
    /// The function to be executed when the command is dispatched.
    /// </summary>
    /// <param name="command">The command itself.</param>
    /// <param name="arguments">The arguments supplied to the command, ready for parsing.</param>
    delegate void HandlerDelegate(string command, string arguments);

    /// <summary>
    /// Gets a <see cref="HandlerDelegate"/> which will be called when the command is dispatched.
    /// </summary>
    HandlerDelegate Handler { get; }

    /// <summary>
    /// Gets the help message for this command.
    /// </summary>
    string HelpMessage { get; }

    /// <summary>
    /// Gets a value indicating whether if this command should be shown in the help output.
    /// </summary>
    bool ShowInHelp { get; }

    /// <summary>
    /// Gets the display order of this command. Defaults to alphabetical ordering.
    /// </summary>
    int DisplayOrder { get; }

    /// <summary>
    /// Gets a value indicating whether this command is permitted to run in macros. Defaults to true.
    /// </summary>
    bool AllowedInMacros { get; }
}
