using Dalamud.Game.Text;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ContextMenu;

/// <summary>
/// A menu item that can be added to a context menu.
/// </summary>
public sealed record MenuItem : IMenuItem
{
    /// <inheritdoc/>
    public ReadOnlySeString Name { get; set; }

    /// <inheritdoc/>
    public SeIconChar? Prefix { get; set; }

    /// <inheritdoc/>
    public char? PrefixChar
    {
        set
        {
            if (value is { } prefix)
            {
                if (!char.IsAsciiLetterUpper(prefix))
                    throw new ArgumentException("Prefix must be an uppercase letter", nameof(value));

                this.Prefix = SeIconChar.BoxedLetterA + prefix - 'A';
            }
            else
            {
                this.Prefix = null;
            }
        }
    }

    /// <inheritdoc/>
    public ushort PrefixColor { get; set; }

    /// <inheritdoc/>
    public bool UseDefaultPrefix { get; set; }

    /// <inheritdoc/>
    public Action<IMenuItemClickedArgs>? OnClicked { get; set; }

    /// <inheritdoc/>
    public int Priority { get; set; }

    /// <inheritdoc/>
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc/>
    public bool IsSubmenu { get; set; }

    /// <inheritdoc/>
    public bool IsReturn { get; set; }
}
