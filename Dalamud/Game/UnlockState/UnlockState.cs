using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Dalamud.Data;
using Dalamud.Hooking;
using Dalamud.Logging.Internal;
using Dalamud.Plugin.Services;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.Exd;

using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace Dalamud.Game.UnlockState;

/// <summary>
/// This class represents the state of the players unlocks.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal unsafe class UnlockState : IInternalDisposableService, IUnlockState
{
    private static readonly ModuleLog Log = new("UnlockState");

    private readonly UnlockStateAddressResolver address;
    private readonly Hook<PerformMateriaActionMigrationDelegate> performMateriaActionMigrationDelegateHook;
    private readonly ConcurrentDictionary<Type, HashSet<uint>> cachedUnlockedRowIds = [];

    [ServiceManager.ServiceDependency]
    private readonly DataManager dataManager = Service<DataManager>.Get();

    [ServiceManager.ServiceDependency]
    private readonly ClientState.ClientState clientState = Service<ClientState.ClientState>.Get();

    [ServiceManager.ServiceConstructor]
    private UnlockState(TargetSigScanner sigScanner)
    {
        this.address = new UnlockStateAddressResolver();
        this.address.Setup(sigScanner);

        this.clientState.Login += this.OnLogin;
        this.clientState.Logout += this.OnLogout;

        this.performMateriaActionMigrationDelegateHook = Hook<PerformMateriaActionMigrationDelegate>.FromAddress(
            this.address.PerformMateriaActionMigration,
            this.PerformMateriaActionMigrationDetour);

        this.performMateriaActionMigrationDelegateHook.Enable();
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void PerformMateriaActionMigrationDelegate(RaptureHotbarModule* thisPtr);

    /// <inheritdoc/>
    public event EventHandler<RowRef>? Unlock;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.clientState.Login -= this.OnLogin;
        this.clientState.Logout -= this.OnLogout;
        this.performMateriaActionMigrationDelegateHook.Dispose();
    }

    /// <inheritdoc/>
    public bool IsActionUnlocked(Lumina.Excel.Sheets.Action row)
    {
        return this.IsUnlockLinkUnlocked(row.UnlockLink.RowId);
    }

    /// <inheritdoc/>
    public bool IsAetherCurrentUnlocked(AetherCurrent row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return PlayerState.Instance()->IsAetherCurrentUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsAetherCurrentCompFlgSetUnlocked(AetherCurrentCompFlgSet row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return PlayerState.Instance()->IsAetherCurrentZoneComplete(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsAozActionUnlocked(AozAction row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        if (row.RowId == 0 || !row.Action.IsValid)
            return false;

        return UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(row.Action.Value.UnlockLink.RowId);
    }

    /// <inheritdoc/>
    public bool IsBannerBgUnlocked(BannerBg row)
    {
        return row.UnlockCondition.IsValid && this.IsBannerConditionUnlocked(row.UnlockCondition.Value);
    }

    /// <inheritdoc/>
    public bool IsBannerConditionUnlocked(BannerCondition row)
    {
        if (row.RowId == 0)
            return false;

        if (!this.clientState.IsLoggedIn)
            return false;

        var rowPtr = ExdModule.GetBannerConditionByIndex(row.RowId);
        if (rowPtr == null)
            return false;

        return ExdModule.GetBannerConditionUnlockState(rowPtr) == 0;
    }

    /// <inheritdoc/>
    public bool IsBannerDecorationUnlocked(BannerDecoration row)
    {
        return row.UnlockCondition.IsValid && this.IsBannerConditionUnlocked(row.UnlockCondition.Value);
    }

    /// <inheritdoc/>
    public bool IsBannerFacialUnlocked(BannerFacial row)
    {
        return row.UnlockCondition.IsValid && this.IsBannerConditionUnlocked(row.UnlockCondition.Value);
    }

    /// <inheritdoc/>
    public bool IsBannerFrameUnlocked(BannerFrame row)
    {
        return row.UnlockCondition.IsValid && this.IsBannerConditionUnlocked(row.UnlockCondition.Value);
    }

    /// <inheritdoc/>
    public bool IsBannerTimelineUnlocked(BannerTimeline row)
    {
        return row.UnlockCondition.IsValid && this.IsBannerConditionUnlocked(row.UnlockCondition.Value);
    }

    /// <inheritdoc/>
    public bool IsBuddyActionUnlocked(BuddyAction row)
    {
        return this.IsUnlockLinkUnlocked(row.UnlockLink);
    }

    /// <inheritdoc/>
    public bool IsBuddyEquipUnlocked(BuddyEquip row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return UIState.Instance()->Buddy.CompanionInfo.IsBuddyEquipUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsCharaMakeCustomizeUnlocked(CharaMakeCustomize row)
    {
        return row.IsPurchasable && this.IsUnlockLinkUnlocked(row.UnlockLink);
    }

    /// <inheritdoc/>
    public bool IsChocoboTaxiUnlocked(ChocoboTaxi row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return UIState.Instance()->IsChocoboTaxiStandUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsCompanionUnlocked(Companion row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return UIState.Instance()->IsCompanionUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsCraftActionUnlocked(CraftAction row)
    {
        return this.IsUnlockLinkUnlocked(row.QuestRequirement.RowId);
    }

    /// <inheritdoc/>
    public bool IsCSBonusContentTypeUnlocked(CSBonusContentType row)
    {
        return this.IsUnlockLinkUnlocked(row.UnlockLink);
    }

    /// <inheritdoc/>
    public bool IsEmoteUnlocked(Emote row)
    {
        return this.IsUnlockLinkUnlocked(row.UnlockLink);
    }

    /// <inheritdoc/>
    public bool IsGeneralActionUnlocked(GeneralAction row)
    {
        return this.IsUnlockLinkUnlocked(row.UnlockLink);
    }

    /// <inheritdoc/>
    public bool IsGlassesUnlocked(Glasses row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return PlayerState.Instance()->IsGlassesUnlocked((ushort)row.RowId);
    }

    /// <inheritdoc/>
    public bool IsHowToUnlocked(HowTo row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return UIState.Instance()->IsHowToUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsInstanceContentUnlocked(Lumina.Excel.Sheets.InstanceContent row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return UIState.IsInstanceContentUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public unsafe bool IsItemUnlocked(Item item)
    {
        if (item.ItemAction.RowId == 0)
            return false;

        if (!this.clientState.IsLoggedIn)
            return false;

        // To avoid the ExdModule.GetItemRowById call, which can return null if the excel page
        // is not loaded, we're going to imitate the IsItemActionUnlocked call first:
        switch ((ItemActionType)item.ItemAction.Value.Type)
        {
            case ItemActionType.Companion:
                return UIState.Instance()->IsCompanionUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionType.BuddyEquip:
                return UIState.Instance()->Buddy.CompanionInfo.IsBuddyEquipUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionType.Mount:
                return PlayerState.Instance()->IsMountUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionType.SecretRecipeBook:
                return PlayerState.Instance()->IsSecretRecipeBookUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionType.UnlockLink:
                return UIState.Instance()->IsUnlockLinkUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionType.TripleTriadCard when item.AdditionalData.Is<TripleTriadCard>():
                return UIState.Instance()->IsTripleTriadCardUnlocked((ushort)item.AdditionalData.RowId);

            case ItemActionType.FolkloreTome:
                return PlayerState.Instance()->IsFolkloreBookUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionType.OrchestrionRoll when item.AdditionalData.Is<Orchestrion>():
                return PlayerState.Instance()->IsOrchestrionRollUnlocked(item.AdditionalData.RowId);

            case ItemActionType.FramersKit:
                return PlayerState.Instance()->IsFramersKitUnlocked(item.AdditionalData.RowId);

            case ItemActionType.Ornament:
                return PlayerState.Instance()->IsOrnamentUnlocked(item.ItemAction.Value.Data[0]);

            case ItemActionType.Glasses:
                return PlayerState.Instance()->IsGlassesUnlocked((ushort)item.AdditionalData.RowId);

            case ItemActionType.CompanySealVouchers:
                return false;
        }

        var row = ExdModule.GetItemRowById(item.RowId);
        return row != null && UIState.Instance()->IsItemActionUnlocked(row) == 1;
    }

    /// <inheritdoc/>
    public bool IsMcGuffinUnlocked(McGuffin row)
    {
        return PlayerState.Instance()->IsMcGuffinUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsMJILandmarkUnlocked(MJILandmark row)
    {
        return this.IsUnlockLinkUnlocked(row.UnlockLink);
    }

    /// <inheritdoc/>
    public bool IsMountUnlocked(Mount row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return PlayerState.Instance()->IsMountUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsNotebookDivisionUnlocked(NotebookDivision row)
    {
        return this.IsUnlockLinkUnlocked(row.QuestUnlock.RowId);
    }

    /// <inheritdoc/>
    public bool IsOrchestrionUnlocked(Orchestrion row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return PlayerState.Instance()->IsOrchestrionRollUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsOrnamentUnlocked(Ornament row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return PlayerState.Instance()->IsOrnamentUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsPerformUnlocked(Perform row)
    {
        return this.IsUnlockLinkUnlocked((uint)row.UnlockLink);
    }

    /// <inheritdoc/>
    public bool IsPublicContentUnlocked(PublicContent row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return UIState.IsPublicContentUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsSecretRecipeBookUnlocked(SecretRecipeBook row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return PlayerState.Instance()->IsSecretRecipeBookUnlocked(row.RowId);
    }

    /// <inheritdoc/>
    public bool IsTraitUnlocked(Trait row)
    {
        return this.IsUnlockLinkUnlocked(row.Quest.RowId);
    }

    /// <inheritdoc/>
    public bool IsTripleTriadCardUnlocked(TripleTriadCard row)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        return UIState.Instance()->IsTripleTriadCardUnlocked((ushort)row.RowId);
    }

    /// <inheritdoc/>
    public bool IsItemUnlockable(Item item)
    {
        if (item.ItemAction.RowId == 0)
            return false;

        return (ItemActionType)item.ItemAction.Value.Type is
            ItemActionType.Companion or
            ItemActionType.BuddyEquip or
            ItemActionType.Mount or
            ItemActionType.SecretRecipeBook or
            ItemActionType.UnlockLink or
            ItemActionType.TripleTriadCard or
            ItemActionType.FolkloreTome or
            ItemActionType.OrchestrionRoll or
            ItemActionType.FramersKit or
            ItemActionType.Ornament or
            ItemActionType.Glasses;
    }

    /// <inheritdoc/>
    public bool IsRowRefUnlocked<T>(RowRef<T> rowRef) where T : struct, IExcelRow<T>
    {
        return this.IsRowRefUnlocked((RowRef)rowRef);
    }

    /// <inheritdoc/>
    public bool IsRowRefUnlocked(RowRef rowRef)
    {
        if (!this.clientState.IsLoggedIn || rowRef.IsUntyped)
            return false;

        if (rowRef.TryGetValue<AetherCurrent>(out var aetherCurrentRow))
            return this.IsAetherCurrentUnlocked(aetherCurrentRow);

        if (rowRef.TryGetValue<AetherCurrentCompFlgSet>(out var aetherCurrentCompFlgSetRow))
            return this.IsAetherCurrentCompFlgSetUnlocked(aetherCurrentCompFlgSetRow);

        if (rowRef.TryGetValue<AozAction>(out var aozActionRow))
            return this.IsAozActionUnlocked(aozActionRow);

        if (rowRef.TryGetValue<BuddyEquip>(out var buddyEquipRow))
            return this.IsBuddyEquipUnlocked(buddyEquipRow);

        if (rowRef.TryGetValue<Companion>(out var companionRow))
            return this.IsCompanionUnlocked(companionRow);

        if (rowRef.TryGetValue<Glasses>(out var glassesRow))
            return this.IsGlassesUnlocked(glassesRow);

        if (rowRef.TryGetValue<Mount>(out var mountRow))
            return this.IsMountUnlocked(mountRow);

        if (rowRef.TryGetValue<SecretRecipeBook>(out var secretRecipeBookRow))
            return this.IsSecretRecipeBookUnlocked(secretRecipeBookRow);

        if (rowRef.TryGetValue<TripleTriadCard>(out var tripleTriadCardRow))
            return this.IsTripleTriadCardUnlocked(tripleTriadCardRow);

        if (rowRef.TryGetValue<Orchestrion>(out var orchestrionRow))
            return this.IsOrchestrionUnlocked(orchestrionRow);

        if (rowRef.TryGetValue<Ornament>(out var ornamentRow))
            return this.IsOrnamentUnlocked(ornamentRow);

        if (rowRef.TryGetValue<HowTo>(out var howToRow))
            return this.IsHowToUnlocked(howToRow);

        if (rowRef.TryGetValue<ChocoboTaxi>(out var chocoboTaxiRow))
            return this.IsChocoboTaxiUnlocked(chocoboTaxiRow);

        if (rowRef.TryGetValue<Lumina.Excel.Sheets.InstanceContent>(out var instanceContentRow))
            return this.IsInstanceContentUnlocked(instanceContentRow);

        if (rowRef.TryGetValue<PublicContent>(out var publicContentRow))
            return this.IsPublicContentUnlocked(publicContentRow);

        if (rowRef.TryGetValue<Lumina.Excel.Sheets.Action>(out var actionRow))
            return this.IsActionUnlocked(actionRow);

        if (rowRef.TryGetValue<GeneralAction>(out var generalActionRow))
            return this.IsGeneralActionUnlocked(generalActionRow);

        if (rowRef.TryGetValue<BuddyAction>(out var buddyActionRow))
            return this.IsBuddyActionUnlocked(buddyActionRow);

        if (rowRef.TryGetValue<CraftAction>(out var craftActionRow))
            return this.IsCraftActionUnlocked(craftActionRow);

        if (rowRef.TryGetValue<Emote>(out var emoteRow))
            return this.IsEmoteUnlocked(emoteRow);

        if (rowRef.TryGetValue<Perform>(out var performRow))
            return this.IsPerformUnlocked(performRow);

        if (rowRef.TryGetValue<MJILandmark>(out var mjiLandmarkRow))
            return this.IsMJILandmarkUnlocked(mjiLandmarkRow);

        if (rowRef.TryGetValue<CSBonusContentType>(out var csBonusContentTypeRow))
            return this.IsCSBonusContentTypeUnlocked(csBonusContentTypeRow);

        if (rowRef.TryGetValue<NotebookDivision>(out var notebookDivisionRow))
            return this.IsNotebookDivisionUnlocked(notebookDivisionRow);

        if (rowRef.TryGetValue<Trait>(out var traitRow))
            return this.IsTraitUnlocked(traitRow);

        if (rowRef.TryGetValue<CharaMakeCustomize>(out var charaMakeCustomizeRow))
            return this.IsCharaMakeCustomizeUnlocked(charaMakeCustomizeRow);

        if (rowRef.TryGetValue<BannerCondition>(out var bannerConditionRow))
            return this.IsBannerConditionUnlocked(bannerConditionRow);

        if (rowRef.TryGetValue<BannerBg>(out var bannerBgRow))
            return this.IsBannerBgUnlocked(bannerBgRow);

        if (rowRef.TryGetValue<BannerFrame>(out var bannerFrameRow))
            return this.IsBannerFrameUnlocked(bannerFrameRow);

        if (rowRef.TryGetValue<BannerDecoration>(out var bannerDecorationRow))
            return this.IsBannerDecorationUnlocked(bannerDecorationRow);

        if (rowRef.TryGetValue<BannerFacial>(out var bannerFacialRow))
            return this.IsBannerFacialUnlocked(bannerFacialRow);

        if (rowRef.TryGetValue<BannerTimeline>(out var bannerTimelineRow))
            return this.IsBannerTimelineUnlocked(bannerTimelineRow);

        if (rowRef.TryGetValue<McGuffin>(out var mcGuffinRow))
            return this.IsMcGuffinUnlocked(mcGuffinRow);

        if (rowRef.TryGetValue<Item>(out var itemRow))
            return this.IsItemUnlocked(itemRow);

        return false;
    }

    /// <inheritdoc/>
    public bool IsUnlockLinkUnlocked(ushort unlockLink)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        if (unlockLink == 0)
            return false;

        return UIState.Instance()->IsUnlockLinkUnlocked(unlockLink);
    }

    /// <inheritdoc/>
    public bool IsUnlockLinkUnlocked(uint unlockLink)
    {
        if (!this.clientState.IsLoggedIn)
            return false;

        if (unlockLink == 0)
            return false;

        return UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(unlockLink);
    }

    private void OnLogin()
    {
        try
        {
            this.UpdateUnlocks(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during initial unlock check");
        }
    }

    private void OnLogout(int type, int code)
    {
        this.cachedUnlockedRowIds.Clear();
    }

    private void PerformMateriaActionMigrationDetour(RaptureHotbarModule* thisPtr)
    {
        try
        {
            this.UpdateUnlocks(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during unlock check");
        }

        this.performMateriaActionMigrationDelegateHook.Original(thisPtr);
    }

    private void UpdateUnlocks(bool fireEvent)
    {
        if (!this.clientState.IsLoggedIn)
            return;

        this.UpdateUnlocksForSheet<AetherCurrent>(fireEvent);
        this.UpdateUnlocksForSheet<AetherCurrentCompFlgSet>(fireEvent);
        this.UpdateUnlocksForSheet<AozAction>(fireEvent);
        this.UpdateUnlocksForSheet<BuddyEquip>(fireEvent);
        this.UpdateUnlocksForSheet<Companion>(fireEvent);
        this.UpdateUnlocksForSheet<Glasses>(fireEvent);
        this.UpdateUnlocksForSheet<Mount>(fireEvent);
        this.UpdateUnlocksForSheet<SecretRecipeBook>(fireEvent);
        this.UpdateUnlocksForSheet<TripleTriadCard>(fireEvent);
        this.UpdateUnlocksForSheet<Orchestrion>(fireEvent);
        this.UpdateUnlocksForSheet<Ornament>(fireEvent);
        this.UpdateUnlocksForSheet<HowTo>(fireEvent);
        this.UpdateUnlocksForSheet<ChocoboTaxi>(fireEvent);
        this.UpdateUnlocksForSheet<Lumina.Excel.Sheets.InstanceContent>(fireEvent);
        this.UpdateUnlocksForSheet<PublicContent>(fireEvent);
        this.UpdateUnlocksForSheet<Lumina.Excel.Sheets.Action>(fireEvent);
        this.UpdateUnlocksForSheet<GeneralAction>(fireEvent);
        this.UpdateUnlocksForSheet<BuddyAction>(fireEvent);
        this.UpdateUnlocksForSheet<CraftAction>(fireEvent);
        this.UpdateUnlocksForSheet<Emote>(fireEvent);
        this.UpdateUnlocksForSheet<Perform>(fireEvent);
        this.UpdateUnlocksForSheet<MJILandmark>(fireEvent);
        this.UpdateUnlocksForSheet<CSBonusContentType>(fireEvent);
        this.UpdateUnlocksForSheet<NotebookDivision>(fireEvent);
        this.UpdateUnlocksForSheet<Trait>(fireEvent);
        this.UpdateUnlocksForSheet<CharaMakeCustomize>(fireEvent);
        this.UpdateUnlocksForSheet<BannerCondition>(fireEvent);
        this.UpdateUnlocksForSheet<BannerBg>(fireEvent);
        this.UpdateUnlocksForSheet<BannerFrame>(fireEvent);
        this.UpdateUnlocksForSheet<BannerDecoration>(fireEvent);
        this.UpdateUnlocksForSheet<BannerFacial>(fireEvent);
        this.UpdateUnlocksForSheet<BannerTimeline>(fireEvent);
        this.UpdateUnlocksForSheet<McGuffin>(fireEvent);
        this.UpdateUnlocksForSheet<Item>(fireEvent);

        // Not implemented:
        // - DescriptionPage: quite complex
        // - QuestAcceptAdditionCondition: ignored

        // Maybe some day:
        // - FishingSpot
        // - Spearfishing
        // - Adventure (Sightseeing)
        // - Recipes
        // - MinerFolkloreTome
        // - BotanistFolkloreTome
        // - FishingFolkloreTome
        // - Other instances, like VVD?
        // - VVDNotebookContents?
        // - FramersKit (is that just an Item?)
        // - ... more?

        // Probably not happening, because it requires fetching data from server:
        // - Achievements
        // - Titles
        // - Bozjan Field Notes

        // ... and I probably missed something. :D
    }

    private void UpdateUnlocksForSheet<T>(bool fireEvent = true) where T : struct, IExcelRow<T>
    {
        var unlockedRowIds = this.cachedUnlockedRowIds.GetOrAdd(typeof(T), _ => []);

        foreach (var row in this.dataManager.GetExcelSheet<T>())
        {
            if (unlockedRowIds.Contains(row.RowId))
                continue;

            var rowRef = LuminaUtils.CreateRef<T>(row.RowId);

            if (!this.IsRowRefUnlocked(rowRef))
                continue;

            unlockedRowIds.Add(row.RowId);

            if (fireEvent)
            {
                Log.Verbose("Unlock detected: {row}", $"{typeof(T).Name}#{row.RowId}");
                this.Unlock.InvokeSafely(this, (RowRef)rowRef);
            }
        }
    }
}
