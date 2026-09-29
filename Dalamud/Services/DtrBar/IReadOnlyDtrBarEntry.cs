using System.Numerics;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.DtrBar;

/// <summary>
/// Interface representing a read-only entry in the server info bar.
/// </summary>
public interface IReadOnlyDtrBarEntry
{
    /// <summary>
    /// Gets the title of this entry.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets a value indicating whether this entry has a click action.
    /// </summary>
    bool HasClickAction { get; }

    /// <summary>
    /// Gets the text of this entry.
    /// </summary>
    ReadOnlySeString Text { get; }

    /// <summary>
    /// Gets a tooltip to be shown when the user mouses over the dtr entry.
    /// </summary>
    ReadOnlySeString Tooltip { get; }

    /// <summary>
    /// Gets a value indicating whether this entry should be shown.
    /// </summary>
    bool Shown { get; }

    /// <summary>
    /// Gets a value indicating this entry's minimum width.
    /// </summary>
    ushort MinimumWidth { get; }

    /// <summary>
    /// Gets a value indicating whether the user has hidden this entry from view through the Dalamud settings.
    /// </summary>
    bool UserHidden { get; }

    /// <summary>
    /// Gets an action to be invoked when the user clicks on the dtr entry.
    /// </summary>
    Action<DtrInteractionEvent>? OnClick { get; }

    /// <summary>
    /// Gets the axis-aligned bounding box of this entry, in screen coordinates.
    /// </summary>
    (Vector2 Min, Vector2 Max) ScreenBounds { get; }
}
