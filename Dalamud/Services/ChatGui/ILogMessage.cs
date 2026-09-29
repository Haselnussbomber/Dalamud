using System.Collections.Generic;

using Dalamud.Services.SeStringEvaluator;

using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ChatGui;

/// <summary>
/// Interface representing a log message.
/// </summary>
public interface ILogMessage : IEquatable<ILogMessage>
{
    /// <summary>
    /// Gets the address of the log message in memory.
    /// </summary>
    nint Address { get; }

    /// <summary>
    /// Gets the ID of this log message.
    /// </summary>
    uint LogMessageId { get; }

    /// <summary>
    /// Gets the GameData associated with this log message.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.LogMessage> GameData { get; }

    /// <summary>
    /// Gets the entity that is the source of this log message, if any.
    /// </summary>
    ILogMessageEntity? SourceEntity { get; }

    /// <summary>
    /// Gets the entity that is the target of this log message, if any.
    /// </summary>
    ILogMessageEntity? TargetEntity { get; }

    /// <summary>
    /// Gets the number of parameters.
    /// </summary>
    int ParameterCount { get; }

    /// <summary>
    /// Gets a list containing the parameters. The returned object is only valid during the <see cref="IChatGui.LogMessage"/> event and must not be accessed after returning from it.
    /// </summary>
    IReadOnlyList<SeStringParameter> Parameters { get; }

    /// <summary>
    /// Gets a value indicating whether the message is handled and will not appear in chat.
    /// </summary>
    bool IsHandled { get; }

    /// <summary>
    /// Marks this message as handled (<see cref="IsHandled"/> = <see langword="true"/>) and prevents it from appearing.
    /// </summary>
    void PreventOriginal();

    /// <summary>
    /// Retrieves the value of a parameter for the log message if it is an int.
    /// </summary>
    /// <param name="index">The index of the parameter to retrieve.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns><see langword="true"/> if the parameter was retrieved successfully.</returns>
    bool TryGetIntParameter(int index, out int value);

    /// <summary>
    /// Retrieves the value of a parameter for the log message if it is a string.
    /// </summary>
    /// <param name="index">The index of the parameter to retrieve.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns><see langword="true"/> if the parameter was retrieved successfully.</returns>
    bool TryGetStringParameter(int index, out ReadOnlySeString value);

    /// <summary>
    /// Formats this log message into an approximation of the string that will eventually be shown in the log.
    /// </summary>
    /// <remarks>This can cause side effects such as playing sound effects and thus should only be used for debugging.</remarks>
    /// <returns>The formatted string.</returns>
    ReadOnlySeString FormatLogMessageForDebugging();
}
