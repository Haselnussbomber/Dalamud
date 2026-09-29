using Dalamud.Interface.Textures;

namespace Dalamud.Services.TitleScreenMenu;

/// <summary>
/// A interface representing a read only entry in the title screen menu.
/// </summary>
public interface IReadOnlyTitleScreenMenuEntry
{
    /// <summary>
    /// Gets the priority of this entry.
    /// </summary>
    ulong Priority { get; }

    /// <summary>
    /// Gets the name of this entry.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the texture of this entry.
    /// </summary>
    ISharedImmediateTexture Texture { get; }
}
