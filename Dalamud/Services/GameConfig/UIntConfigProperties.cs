namespace Dalamud.Services.GameConfig;

/// <summary>
/// Represents a uint configuration property.
/// </summary>
/// <param name="Default">The default value.</param>
/// <param name="Minimum">The minimum value.</param>
/// <param name="Maximum">The maximum value.</param>
public record UIntConfigProperties(uint Default, uint Minimum, uint Maximum);
