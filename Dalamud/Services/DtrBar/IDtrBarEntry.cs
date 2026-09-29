using Lumina.Text.ReadOnly;

namespace Dalamud.Services.DtrBar;

/// <summary>
/// Interface representing an entry in the server info bar.
/// </summary>
public interface IDtrBarEntry : IReadOnlyDtrBarEntry, IDisposable
{
    /// <summary>
    /// Gets or sets the text of this entry.
    /// </summary>
    new ReadOnlySeString Text { get; set; }

    /// <summary>
    /// Gets or sets a tooltip to be shown when the user mouses over the dtr entry.
    /// </summary>
    new ReadOnlySeString Tooltip { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this entry is visible.
    /// </summary>
    new bool Shown { get; set; }

    /// <summary>
    /// Gets or sets a value specifying the requested minimum width to make this entry.
    /// </summary>
    new ushort MinimumWidth { get; set; }

    /// <summary>
    /// Gets or sets an action to be invoked when the user clicks on the dtr entry.
    /// </summary>
    new Action<DtrInteractionEvent>? OnClick { get; set; }

    /// <summary>
    /// Remove this entry from the bar.
    /// You will need to re-acquire it from DtrBar to reuse it.
    /// </summary>
    void Remove();
}
