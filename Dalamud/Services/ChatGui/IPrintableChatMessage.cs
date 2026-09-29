namespace Dalamud.Services.ChatGui;

/// <summary>
/// This interface represents a single chat message sent from a plugin.
/// </summary>
public interface IPrintableChatMessage : IChatMessage
{
    /// <summary>
    /// Gets or sets a value indicating whether new message sounds should be silenced or not.
    /// </summary>
    bool Silent { get; set; }
}
