namespace Dalamud.Services.DtrBar;

/// <summary>
/// Modifier keys that can be held during a mouse click event.
/// </summary>
[Flags]
public enum ClickModifierKeys
{
    /// <summary>
    /// No modifiers were present.
    /// </summary>
    None = 0,

    /// <summary>
    /// The CTRL key was held.
    /// </summary>
    Ctrl = 1 << 0,

    /// <summary>
    /// The ALT key was held.
    /// </summary>
    Alt = 1 << 1,

    /// <summary>
    /// The SHIFT key was held.
    /// </summary>
    Shift = 1 << 2,
}
