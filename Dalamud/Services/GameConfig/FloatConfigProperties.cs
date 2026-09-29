namespace Dalamud.Services.GameConfig;

/// <summary>
/// Represents a floating point configuration property.
/// </summary>
/// <param name="Default">The default value.</param>
/// <param name="Minimum">The minimum value.</param>
/// <param name="Maximum">The maximum value.</param>
public record FloatConfigProperties(float Default, float Minimum, float Maximum);
