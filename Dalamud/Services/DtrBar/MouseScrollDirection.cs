namespace Dalamud.Services.DtrBar;

/// <summary>
/// Possible directions for scroll wheel events.
/// </summary>
public enum MouseScrollDirection
{
    /// <summary>
    /// No scrolling.
    /// </summary>
    None = 0,

    /// <summary>
    /// A scroll up event.
    /// </summary>
    Up = 1,

    /// <summary>
    /// A scroll down event.
    /// </summary>
    Down = -1,
}
