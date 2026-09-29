using Dalamud.Game.Gui;

namespace Dalamud.Services.GameGui;

/// <inheritdoc/>
internal class HoveredAction : IHoveredAction
{
    /// <inheritdoc/>
    public uint BaseActionId { get; set; } = 0;

    /// <inheritdoc/>
    public uint ActionId { get; set; } = 0;

    /// <inheritdoc/>
    public DetailKind DetailKind { get; set; } = DetailKind.None;
}
