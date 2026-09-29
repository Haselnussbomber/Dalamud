using Lumina.Text.ReadOnly;

namespace Dalamud.Services.GameConfig;

/// <summary>
/// Represents a string configuration property.
/// </summary>
/// <param name="Default">The default value.</param>
public record StringConfigProperties(ReadOnlySeString Default);
