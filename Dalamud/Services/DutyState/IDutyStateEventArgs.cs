using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace Dalamud.Services.DutyState;

/// <summary>
/// Interface for providing event data when the duty state changed.
/// </summary>
public interface IDutyStateEventArgs
{
    /// <summary>
    /// Gets a RowRef for the TerritoryType at the time the event was fired.
    /// </summary>
    RowRef<TerritoryType> TerritoryType { get; }

    /// <summary>
    /// Gets a RowRef for the ContentFinderCondition at the time the event was fired.
    /// </summary>
    RowRef<ContentFinderCondition> ContentFinderCondition { get; }

    /// <summary>
    /// Gets the EventHandler id for which this event was fired.
    /// </summary>
    uint EventHandlerId { get; }
}
