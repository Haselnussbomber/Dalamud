namespace Dalamud.Services.Marketboard.Structures;

/// <summary>
/// An interface that represents the materia slotted to an <see cref="IMarketBoardItemListing"/>.
/// </summary>
public interface IItemMateria
{
    /// <summary>
    /// Gets the materia index.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Gets the materia ID.
    /// </summary>
    int MateriaId { get; }
}
