using Lumina.Excel.Sheets;

namespace Dalamud.Game;

/// <summary>
/// Enum for <see cref="ItemAction.Type"/>.
/// </summary>
public enum ItemActionType : ushort
{
    Companion = 853,
    BuddyEquip = 1013,
    Mount = 1322,
    SecretRecipeBook = 2136,
    UnlockLink = 2633, // Riding Maps, Blumage Totems, Emotes, Hairstyles...
    TripleTriadCard = 3357,
    FolkloreTome = 4107,
    OrchestrionRoll = 25183,
    FramersKit = 29459,
    FieldNotes = 19743, // Bozjan Field Notes (server side, but cached)
    Ornament = 20086,
    Glasses = 37312,
    CompanySealVouchers = 41120, // can use = is in grand company, is unlocked = always false
}
