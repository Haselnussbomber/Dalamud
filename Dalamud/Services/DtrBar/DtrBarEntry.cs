using System.Numerics;

using Dalamud.Configuration.Internal;
using Dalamud.Plugin.Internal.Types;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.DtrBar;

/// <summary>
/// Class representing an entry in the server info bar.
/// </summary>
internal sealed unsafe class DtrBarEntry : IDisposable, IDtrBarEntry
{
    private readonly DalamudConfiguration configuration;

    private bool shownBacking = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DtrBarEntry"/> class.
    /// </summary>
    /// <param name="configuration">Dalamud configuration, used to check if the entry is hidden by the user.</param>
    /// <param name="title">The title of the bar entry.</param>
    /// <param name="textNode">The corresponding text node.</param>
    internal DtrBarEntry(DalamudConfiguration configuration, string title, AtkTextNode* textNode)
    {
        this.configuration = configuration;
        this.Title = title;
        this.TextNode = textNode;
    }

    /// <inheritdoc/>
    public string Title { get; init; }

    /// <inheritdoc cref="IDtrBarEntry.Text" />
    public ReadOnlySeString Text
    {
        get;
        set
        {
            field = value;
            this.Dirty = true;
        }
    }

    /// <inheritdoc cref="IDtrBarEntry.Tooltip" />
    public ReadOnlySeString Tooltip { get; set; }

    /// <inheritdoc cref="MinimumWidth" />
    public ushort MinimumWidth
    {
        get;
        set
        {
            field = value;
            if (this.TextNode is not null)
            {
                if (this.TextNode->GetWidth() < value)
                {
                    this.TextNode->SetWidth(value);
                }
            }

            this.Dirty = true;
        }
    }

    /// <inheritdoc/>
    public Action<DtrInteractionEvent>? OnClick { get; set; }

    /// <inheritdoc/>
    public bool HasClickAction => this.OnClick != null;

    /// <inheritdoc cref="IDtrBarEntry.Shown" />
    public bool Shown
    {
        get => this.shownBacking;
        set
        {
            if (value != this.shownBacking)
            {
                this.shownBacking = value;
                this.Dirty = true;
            }
        }
    }

    /// <inheritdoc/>
    [Api16ToDo("Maybe make this config scoped to internal name?")]
    public bool UserHidden => this.configuration.DtrIgnore?.Contains(this.Title) ?? false;

    /// <inheritdoc/>
    public (Vector2 Min, Vector2 Max) ScreenBounds
        => this.TextNode switch
        {
            null => default,
            var node => node->IsVisible()
                            ? (new(node->ScreenX, node->ScreenY),
                                  new(node->ScreenX + node->GetWidth(), node->ScreenY + node->GetHeight()))
                            : default,
        };

    /// <summary>
    /// Gets or sets the internal text node of this entry.
    /// </summary>
    internal AtkTextNode* TextNode { get; set; }

    /// <summary>
    /// Gets or sets the storage for the text of this entry.
    /// </summary>
    internal Utf8String* Storage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this entry should be removed.
    /// </summary>
    internal bool ShouldBeRemoved { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this entry is dirty.
    /// </summary>
    internal bool Dirty { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this entry has just been added.
    /// </summary>
    internal bool Added { get; set; }

    /// <summary>
    /// Gets or sets the plugin that owns this entry.
    /// </summary>
    internal LocalPlugin? OwnerPlugin { get; set; }

    /// <summary>
    /// Remove this entry from the bar.
    /// You will need to re-acquire it from DtrBar to reuse it.
    /// </summary>
    public void Remove()
    {
        this.ShouldBeRemoved = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Remove();
    }
}
