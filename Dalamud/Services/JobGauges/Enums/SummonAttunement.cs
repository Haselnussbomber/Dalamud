using Dalamud.Services.JobGauges.Types;

namespace Dalamud.Services.JobGauges.Enums;

/// <summary>
/// Enum representing the current attunement of a  summoner.
/// </summary>
public enum SummonAttunement
{
    /// <summary>
    /// No attunement.
    /// </summary>
    None = 0,

    /// <summary>
    /// Attuned to the summon Ifrit.
    /// Same as <see cref="SMNGauge.IsIfritAttuned"/>.
    /// </summary>
    Ifrit = 1,

    /// <summary>
    /// Attuned to the summon Titan.
    /// Same as <see cref="SMNGauge.IsTitanAttuned"/>.
    /// </summary>
    Titan = 2,

    /// <summary>
    /// Attuned to the summon Garuda.
    /// Same as <see cref="SMNGauge.IsGarudaAttuned"/>.
    /// </summary>
    Garuda = 3,
}
