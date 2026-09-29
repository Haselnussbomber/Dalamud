using System.Collections.Generic;
using System.Linq;

using Dalamud.Services.DataManager;

using FFXIVClientStructs.FFXIV.Client.Game.Network;

using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace Dalamud.Services.PartyFinderGui.Types;

/// <summary>
/// A single listing in party finder.
/// </summary>
internal unsafe class PartyFinderListing : IPartyFinderListing
{
    private readonly CrossRealmListingSegmentPacket.CrossRealmListing* listing;
    private readonly PartyFinderSlot[] slots;
    private readonly byte[] jobsPresent;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyFinderListing"/> class.
    /// </summary>
    /// <param name="listing">The interop listing data.</param>
    internal PartyFinderListing(CrossRealmListingSegmentPacket.CrossRealmListing* listing)
    {
        this.listing = listing;

        this.slots = new PartyFinderSlot[listing->SlotFlags.Length];
        for (var i = 0; i < this.slots.Length; i++)
            this.slots[i] = new PartyFinderSlot(listing->SlotFlags[i]);

        this.jobsPresent = listing->JobsPresent.ToArray();
        this.JobsPresent = this.jobsPresent
                                  .Select(id => LuminaUtils.CreateRef<ClassJob>(id))
                                  .ToArray();
    }

    /// <inheritdoc/>
    public ulong Id => this.listing->ListingId;

    /// <inheritdoc/>
    public ulong ContentId => this.listing->ContentId;

    /// <inheritdoc/>
    public ReadOnlySeString Name => this.listing->Name;

    /// <inheritdoc/>
    public ReadOnlySeString Description => this.listing->Description;

    /// <inheritdoc/>
    public RowRef<World> World => LuminaUtils.CreateRef<World>(this.listing->WorldId);

    /// <inheritdoc/>
    public RowRef<World> HomeWorld => LuminaUtils.CreateRef<World>(this.listing->HomeWorldId);

    /// <inheritdoc/>
    public RowRef<World> CurrentWorld => LuminaUtils.CreateRef<World>(this.listing->CurrentWorldId);

    /// <inheritdoc/>
    public DutyCategory Category => (DutyCategory)this.listing->Category;

    /// <inheritdoc/>
    public ushort RawDuty => this.listing->Duty;

    /// <inheritdoc/>
    public RowRef<ContentFinderCondition> Duty => LuminaUtils.CreateRef<ContentFinderCondition>(this.listing->Duty);

    /// <inheritdoc/>
    public DutyType DutyType => (DutyType)this.listing->DutyType;

    /// <inheritdoc/>
    public bool BeginnersWelcome => this.listing->BeginnersWelcome == 1;

    /// <inheritdoc/>
    public ushort SecondsRemaining => this.listing->TimeLeft;

    /// <inheritdoc/>
    public ushort MinimumItemLevel => this.listing->AvgItemLv;

    /// <inheritdoc/>
    public byte Parties => this.listing->NumberOfParties;

    /// <inheritdoc/>
    public byte SlotsAvailable => this.listing->TotalSlots;

    /// <inheritdoc/>
    public byte SlotsFilled => this.listing->SlotsFilled;

    /// <inheritdoc/>
    public int LastPatchHotfixTimestamp => this.listing->LastPatchHotfixTimestamp;

    /// <inheritdoc/>
    public IReadOnlyCollection<IPartyFinderSlot> Slots => this.slots;

    /// <inheritdoc/>
    public ObjectiveFlags Objective => (ObjectiveFlags)this.listing->Objective;

    /// <inheritdoc/>
    public ConditionFlags Conditions => (ConditionFlags)this.listing->CompletionStatus;

    /// <inheritdoc/>
    public DutyFinderSettingsFlags DutyFinderSettings => (DutyFinderSettingsFlags)this.listing->DutyFinderSettings;

    /// <inheritdoc/>
    public LootRuleFlags LootRules => (LootRuleFlags)this.listing->LootRule;

    /// <inheritdoc/>
    public SearchAreaFlags SearchArea => (SearchAreaFlags)this.listing->JoinConditionFlags;

    /// <inheritdoc/>
    public IReadOnlyCollection<byte> RawJobsPresent => this.jobsPresent;

    /// <inheritdoc/>
    public IReadOnlyCollection<RowRef<ClassJob>> JobsPresent { get; }

    #region Indexers

    /// <inheritdoc/>
    public bool this[ObjectiveFlags flag] => this.listing->Objective == 0 || (this.listing->Objective & (byte)flag) != 0;

    /// <inheritdoc/>
    public bool this[ConditionFlags flag] => this.listing->CompletionStatus == 0 || (this.listing->CompletionStatus & (byte)flag) != 0;

    /// <inheritdoc/>
    public bool this[DutyFinderSettingsFlags flag] => this.listing->DutyFinderSettings == 0 || (this.listing->DutyFinderSettings & (byte)flag) != 0;

    /// <inheritdoc/>
    public bool this[LootRuleFlags flag] => this.listing->LootRule == 0 || (this.listing->LootRule & (byte)flag) != 0;

    /// <inheritdoc/>
    public bool this[SearchAreaFlags flag] => this.listing->JoinConditionFlags == 0 || (this.listing->JoinConditionFlags & (byte)flag) != 0;

    #endregion
}
