namespace Dalamud.Services.NamePlateGui;

/// <summary>
/// Contains information related to the pending nameplate data update. This is only valid for a single frame and should
/// not be kept across frames.
/// </summary>
public interface INamePlateUpdateContext
{
    /// <summary>
    /// Gets the number of active nameplates. The actual number visible may be lower than this in cases where some
    /// nameplates are hidden by default (based on in-game "Display Name Settings" and so on).
    /// </summary>
    int ActiveNamePlateCount { get; }

    /// <summary>
    /// Gets a value indicating whether a forced re-draw is currently being performed.
    /// </summary>
    bool IsFullUpdate { get; }

    /// <summary>
    /// Gets the address of the NamePlate addon.
    /// </summary>
    nint AddonAddress { get; }

    /// <summary>
    /// Gets the address of the NamePlate addon's number array data container.
    /// </summary>
    nint NumberArrayDataAddress { get; }

    /// <summary>
    /// Gets the address of the NamePlate addon's string array data container.
    /// </summary>
    nint StringArrayDataAddress { get; }

    /// <summary>
    /// Gets the address of the first entry in the NamePlate addon's int array.
    /// </summary>
    nint NumberArrayDataEntryAddress { get; }
}
