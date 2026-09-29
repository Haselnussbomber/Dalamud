namespace Dalamud.Services.CommandManager;

/// <summary>
/// This class describes a registered command.
/// </summary>
public sealed class CommandInfo : IReadOnlyCommandInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandInfo"/> class.
    /// Create a new CommandInfo with the provided handler.
    /// </summary>
    /// <param name="handler">The method to call when the command is run.</param>
    public CommandInfo(IReadOnlyCommandInfo.HandlerDelegate handler)
    {
        this.Handler = handler;
    }

    /// <inheritdoc/>
    public IReadOnlyCommandInfo.HandlerDelegate Handler { get; }

    /// <inheritdoc/>
    public string HelpMessage { get; set; } = string.Empty;

    /// <inheritdoc/>
    public bool ShowInHelp { get; set; } = true;

    /// <inheritdoc/>
    public int DisplayOrder { get; set; } = -1;

    /// <inheritdoc/>
    public bool AllowedInMacros { get; set; } = true;
}
