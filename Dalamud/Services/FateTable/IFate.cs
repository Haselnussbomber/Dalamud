using System.Numerics;

using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace Dalamud.Services.FateTable;

/// <summary>
/// Interface representing a fate entry that can be seen in the current area.
/// </summary>
public interface IFate : IEquatable<IFate>
{
    /// <summary>
    /// Gets the Fate ID of this <see cref="Fate" />.
    /// </summary>
    ushort FateId { get; }

    /// <summary>
    /// Gets game data linked to this Fate.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.Fate> GameData { get; }

    /// <summary>
    /// Gets the time this <see cref="Fate"/> started.
    /// </summary>
    int StartTimeEpoch { get; }

    /// <summary>
    /// Gets how long this <see cref="Fate"/> will run.
    /// </summary>
    short Duration { get; }

    /// <summary>
    /// Gets the remaining time in seconds for this <see cref="Fate"/>.
    /// </summary>
    long TimeRemaining { get; }

    /// <summary>
    /// Gets the displayname of this <see cref="Fate" />.
    /// </summary>
    ReadOnlySeStringSpan Name { get; }

    /// <summary>
    /// Gets the description of this <see cref="Fate" />.
    /// </summary>
    ReadOnlySeStringSpan Description { get; }

    /// <summary>
    /// Gets the objective of this <see cref="Fate" />.
    /// </summary>
    ReadOnlySeStringSpan Objective { get; }

    /// <summary>
    /// Gets the state of this <see cref="Fate"/> (Running, Ended, Failed, Preparation, WaitingForEnd).
    /// </summary>
    FateState State { get; }

    /// <summary>
    /// Gets the hand in count of this <see cref="Fate"/>.
    /// </summary>
    byte HandInCount { get; }

    /// <summary>
    /// Gets the progress amount of this <see cref="Fate"/>.
    /// </summary>
    byte Progress { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Fate"/> has a bonus.
    /// </summary>
    bool HasBonus { get; }

    /// <summary>
    /// Gets the icon id of this <see cref="Fate"/>.
    /// </summary>
    uint IconId { get; }

    /// <summary>
    /// Gets the level of this <see cref="Fate"/>.
    /// </summary>
    byte Level { get; }

    /// <summary>
    /// Gets the max level level of this <see cref="Fate"/>.
    /// </summary>
    byte MaxLevel { get; }

    /// <summary>
    /// Gets the position of this <see cref="Fate"/>.
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// Gets the radius of this <see cref="Fate"/>.
    /// </summary>
    float Radius { get; }

    /// <summary>
    /// Gets the map icon id of this <see cref="Fate"/>.
    /// </summary>
    uint MapIconId { get; }

    /// <summary>
    /// Gets the territory this <see cref="Fate"/> is located in.
    /// </summary>
    RowRef<Lumina.Excel.Sheets.TerritoryType> TerritoryType { get; }

    /// <summary>
    /// Gets the address of this Fate in memory.
    /// </summary>
    nint Address { get; }
}
