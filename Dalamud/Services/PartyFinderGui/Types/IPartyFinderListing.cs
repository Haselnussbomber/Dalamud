using System.Collections.Generic;

using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace Dalamud.Services.PartyFinderGui.Types;

/// <summary>
/// A interface representing a single listing in party finder.
/// </summary>
public interface IPartyFinderListing
{
    /// <summary>
    /// Gets  the objective of this listing.
    /// </summary>
    ObjectiveFlags Objective { get; }

    /// <summary>
    /// Gets the conditions of this listing.
    /// </summary>
    ConditionFlags Conditions { get; }

    /// <summary>
    /// Gets the Duty Finder settings that will be used for this listing.
    /// </summary>
    DutyFinderSettingsFlags DutyFinderSettings { get; }

    /// <summary>
    /// Gets the loot rules that will be used for this listing.
    /// </summary>
    LootRuleFlags LootRules { get; }

    /// <summary>
    /// Gets where this listing is searching. Note that this is also used for denoting alliance raid listings and one
    /// player per job.
    /// </summary>
    SearchAreaFlags SearchArea { get; }

    /// <summary>
    /// Gets a list of player slots that the Party Finder is accepting.
    /// </summary>
    IReadOnlyCollection<IPartyFinderSlot> Slots { get; }

    /// <summary>
    /// Gets a list of the classes/jobs that are currently present in the party.
    /// </summary>
    IReadOnlyCollection<RowRef<ClassJob>> JobsPresent { get; }

    /// <summary>
    /// Gets the ID assigned to this listing by the game's server.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// Gets the player's unique content ID.
    /// </summary>
    ulong ContentId { get; }

    /// <summary>
    /// Gets the name of the player hosting this listing.
    /// </summary>
    ReadOnlySeString Name { get; }

    /// <summary>
    /// Gets the description of this listing as set by the host. May be multiple lines.
    /// </summary>
    ReadOnlySeString Description { get; }

    /// <summary>
    /// Gets the world that this listing was created on.
    /// </summary>
    RowRef<World> World { get; }

    /// <summary>
    /// Gets the home world of the listing's host.
    /// </summary>
    RowRef<World> HomeWorld { get; }

    /// <summary>
    /// Gets the current world of the listing's host.
    /// </summary>
    RowRef<World> CurrentWorld { get; }

    /// <summary>
    /// Gets the Party Finder category this listing is listed under.
    /// </summary>
    DutyCategory Category { get; }

    /// <summary>
    /// Gets the row ID of the duty this listing is for. May be 0 for non-duty listings.
    /// </summary>
    ushort RawDuty { get; }

    /// <summary>
    /// Gets the duty this listing is for. May be null for non-duty listings.
    /// </summary>
    RowRef<ContentFinderCondition> Duty { get; }

    /// <summary>
    /// Gets the type of duty this listing is for.
    /// </summary>
    DutyType DutyType { get; }

    /// <summary>
    /// Gets a value indicating whether if this listing is beginner-friendly. Shown with a sprout icon in-game.
    /// </summary>
    bool BeginnersWelcome { get; }

    /// <summary>
    /// Gets how many seconds this listing will continue to be available for. It may end before this time if the party
    /// fills or the host ends it early.
    /// </summary>
    ushort SecondsRemaining { get; }

    /// <summary>
    /// Gets the minimum item level required to join this listing.
    /// </summary>
    ushort MinimumItemLevel { get; }

    /// <summary>
    /// Gets the number of parties this listing is recruiting for.
    /// </summary>
    byte Parties { get; }

    /// <summary>
    /// Gets the number of player slots this listing is recruiting for.
    /// </summary>
    byte SlotsAvailable { get; }

    /// <summary>
    /// Gets the number of player slots filled.
    /// </summary>
    byte SlotsFilled { get; }

    /// <summary>
    /// Gets the time at which the server this listings is on last restarted for a patch/hotfix.
    /// Probably.
    /// </summary>
    int LastPatchHotfixTimestamp { get; }

    /// <summary>
    /// Gets a list of the class/job IDs that are currently present in the party.
    /// </summary>
    IReadOnlyCollection<byte> RawJobsPresent { get; }

    /// <summary>
    /// Check if the given flag is present.
    /// </summary>
    /// <param name="flag">The flag to check for.</param>
    /// <returns>A value indicating whether the flag is present.</returns>
    bool this[ObjectiveFlags flag] { get; }

    /// <summary>
    /// Check if the given flag is present.
    /// </summary>
    /// <param name="flag">The flag to check for.</param>
    /// <returns>A value indicating whether the flag is present.</returns>
    bool this[ConditionFlags flag] { get; }

    /// <summary>
    /// Check if the given flag is present.
    /// </summary>
    /// <param name="flag">The flag to check for.</param>
    /// <returns>A value indicating whether the flag is present.</returns>
    bool this[DutyFinderSettingsFlags flag] { get; }

    /// <summary>
    /// Check if the given flag is present.
    /// </summary>
    /// <param name="flag">The flag to check for.</param>
    /// <returns>A value indicating whether the flag is present.</returns>
    bool this[LootRuleFlags flag] { get; }

    /// <summary>
    /// Check if the given flag is present.
    /// </summary>
    /// <param name="flag">The flag to check for.</param>
    /// <returns>A value indicating whether the flag is present.</returns>
    bool this[SearchAreaFlags flag] { get; }
}
