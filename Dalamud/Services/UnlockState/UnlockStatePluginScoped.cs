using Dalamud.IoC;
using Dalamud.IoC.Internal;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using AchievementSheet = Lumina.Excel.Sheets.Achievement;
using ActionSheet = Lumina.Excel.Sheets.Action;
using InstanceContentSheet = Lumina.Excel.Sheets.InstanceContent;
using PublicContentSheet = Lumina.Excel.Sheets.PublicContent;

namespace Dalamud.Services.UnlockState;

/// <summary>
/// Plugin-scoped version of a <see cref="UnlockState"/> service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IUnlockState>]
#pragma warning restore SA1015
internal sealed class UnlockStatePluginScoped : IInternalDisposableService, IUnlockState
{
    [ServiceManager.ServiceDependency]
    private readonly UnlockState unlockStateService = Service<UnlockState>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockStatePluginScoped"/> class.
    /// </summary>
    internal UnlockStatePluginScoped()
    {
        this.unlockStateService.Unlock += this.UnlockForward;
    }

    /// <inheritdoc/>
    public event IUnlockState.UnlockDelegate? Unlock;

    /// <inheritdoc/>
    public bool IsAchievementListLoaded => this.unlockStateService.IsAchievementListLoaded;

    /// <inheritdoc/>
    public bool IsTitleListLoaded => this.unlockStateService.IsTitleListLoaded;

    /// <inheritdoc/>
    public bool IsXBMPetListLoaded => this.unlockStateService.IsXBMPetListLoaded;

    /// <inheritdoc/>
    public bool IsAchievementComplete(AchievementSheet row) => this.unlockStateService.IsAchievementComplete(row);

    /// <inheritdoc/>
    public bool IsActionUnlocked(ActionSheet row) => this.unlockStateService.IsActionUnlocked(row);

    /// <inheritdoc/>
    public bool IsAdventureComplete(Adventure row) => this.unlockStateService.IsAdventureComplete(row);

    /// <inheritdoc/>
    public bool IsAetherCurrentCompFlgSetUnlocked(AetherCurrentCompFlgSet row) => this.unlockStateService.IsAetherCurrentCompFlgSetUnlocked(row);

    /// <inheritdoc/>
    public bool IsAetherCurrentUnlocked(AetherCurrent row) => this.unlockStateService.IsAetherCurrentUnlocked(row);

    /// <inheritdoc/>
    public bool IsAozActionUnlocked(AozAction row) => this.unlockStateService.IsAozActionUnlocked(row);

    /// <inheritdoc/>
    public bool IsBannerBgUnlocked(BannerBg row) => this.unlockStateService.IsBannerBgUnlocked(row);

    /// <inheritdoc/>
    public bool IsBannerConditionUnlocked(BannerCondition row) => this.unlockStateService.IsBannerConditionUnlocked(row);

    /// <inheritdoc/>
    public bool IsBannerDecorationUnlocked(BannerDecoration row) => this.unlockStateService.IsBannerDecorationUnlocked(row);

    /// <inheritdoc/>
    public bool IsBannerFacialUnlocked(BannerFacial row) => this.unlockStateService.IsBannerFacialUnlocked(row);

    /// <inheritdoc/>
    public bool IsBannerFrameUnlocked(BannerFrame row) => this.unlockStateService.IsBannerFrameUnlocked(row);

    /// <inheritdoc/>
    public bool IsBannerTimelineUnlocked(BannerTimeline row) => this.unlockStateService.IsBannerTimelineUnlocked(row);

    /// <inheritdoc/>
    public bool IsBuddyActionUnlocked(BuddyAction row) => this.unlockStateService.IsBuddyActionUnlocked(row);

    /// <inheritdoc/>
    public bool IsBuddyEquipUnlocked(BuddyEquip row) => this.unlockStateService.IsBuddyEquipUnlocked(row);

    /// <inheritdoc/>
    public bool IsCharaMakeCustomizeUnlocked(CharaMakeCustomize row) => this.unlockStateService.IsCharaMakeCustomizeUnlocked(row);

    /// <inheritdoc/>
    public bool IsChocoboTaxiStandUnlocked(ChocoboTaxiStand row) => this.unlockStateService.IsChocoboTaxiStandUnlocked(row);

    /// <inheritdoc/>
    public bool IsClassJobUnlocked(ClassJob row) => this.unlockStateService.IsClassJobUnlocked(row);

    /// <inheritdoc/>
    public bool IsCompanionUnlocked(Companion row) => this.unlockStateService.IsCompanionUnlocked(row);

    /// <inheritdoc/>
    public bool IsCraftActionUnlocked(CraftAction row) => this.unlockStateService.IsCraftActionUnlocked(row);

    /// <inheritdoc/>
    public bool IsCSBonusContentTypeUnlocked(CSBonusContentType row) => this.unlockStateService.IsCSBonusContentTypeUnlocked(row);

    /// <inheritdoc/>
    public bool IsEmoteUnlocked(Emote row) => this.unlockStateService.IsEmoteUnlocked(row);

    /// <inheritdoc/>
    public bool IsEmjVoiceNpcUnlocked(EmjVoiceNpc row) => this.unlockStateService.IsEmjVoiceNpcUnlocked(row);

    /// <inheritdoc/>
    public bool IsEmjCostumeUnlocked(EmjCostume row) => this.unlockStateService.IsEmjCostumeUnlocked(row);

    /// <inheritdoc/>
    public bool IsGeneralActionUnlocked(GeneralAction row) => this.unlockStateService.IsGeneralActionUnlocked(row);

    /// <inheritdoc/>
    public bool IsGlassesUnlocked(Glasses row) => this.unlockStateService.IsGlassesUnlocked(row);

    /// <inheritdoc/>
    public bool IsGlassesStyleUnlocked(GlassesStyle row) => this.unlockStateService.IsGlassesStyleUnlocked(row);

    /// <inheritdoc/>
    public bool IsHowToUnlocked(HowTo row) => this.unlockStateService.IsHowToUnlocked(row);

    /// <inheritdoc/>
    public bool IsInstanceContentUnlocked(InstanceContentSheet row) => this.unlockStateService.IsInstanceContentUnlocked(row);

    /// <inheritdoc/>
    public bool IsItemUnlockable(Item row) => this.unlockStateService.IsItemUnlockable(row);

    /// <inheritdoc/>
    public bool IsItemUnlocked(Item row) => this.unlockStateService.IsItemUnlocked(row);

    /// <inheritdoc/>
    public bool IsLeveCompleted(Leve row) => this.unlockStateService.IsLeveCompleted(row);

    /// <inheritdoc/>
    public bool IsMJILandmarkUnlocked(MJILandmark row) => this.unlockStateService.IsMJILandmarkUnlocked(row);

    /// <inheritdoc/>
    public bool IsMKDLoreUnlocked(MKDLore row) => this.unlockStateService.IsMKDLoreUnlocked(row);

    /// <inheritdoc/>
    public bool IsMcGuffinUnlocked(McGuffin row) => this.unlockStateService.IsMcGuffinUnlocked(row);

    /// <inheritdoc/>
    public bool IsMountUnlocked(Mount row) => this.unlockStateService.IsMountUnlocked(row);

    /// <inheritdoc/>
    public bool IsNotebookDivisionUnlocked(NotebookDivision row) => this.unlockStateService.IsNotebookDivisionUnlocked(row);

    /// <inheritdoc/>
    public bool IsOrchestrionUnlocked(Orchestrion row) => this.unlockStateService.IsOrchestrionUnlocked(row);

    /// <inheritdoc/>
    public bool IsOrnamentUnlocked(Ornament row) => this.unlockStateService.IsOrnamentUnlocked(row);

    /// <inheritdoc/>
    public bool IsPerformUnlocked(Perform row) => this.unlockStateService.IsPerformUnlocked(row);

    /// <inheritdoc/>
    public bool IsPublicContentUnlocked(PublicContentSheet row) => this.unlockStateService.IsPublicContentUnlocked(row);

    /// <inheritdoc/>
    public bool IsQuestCompleted(Quest row) => this.unlockStateService.IsQuestCompleted(row);

    /// <inheritdoc/>
    public bool IsRecipeUnlocked(Recipe row) => this.unlockStateService.IsRecipeUnlocked(row);

    /// <inheritdoc/>
    public bool IsRowRefUnlocked(RowRef rowRef) => this.unlockStateService.IsRowRefUnlocked(rowRef);

    /// <inheritdoc/>
    public bool IsRowRefUnlocked<T>(RowRef<T> rowRef) where T : struct, IExcelRow<T> => this.unlockStateService.IsRowRefUnlocked(rowRef);

    /// <inheritdoc/>
    public bool IsSecretRecipeBookUnlocked(SecretRecipeBook row) => this.unlockStateService.IsSecretRecipeBookUnlocked(row);

    /// <inheritdoc/>
    public bool IsTitleUnlocked(Title row) => this.unlockStateService.IsTitleUnlocked(row);

    /// <inheritdoc/>
    public bool IsTraitUnlocked(Trait row) => this.unlockStateService.IsTraitUnlocked(row);

    /// <inheritdoc/>
    public bool IsTripleTriadCardUnlocked(TripleTriadCard row) => this.unlockStateService.IsTripleTriadCardUnlocked(row);

    /// <inheritdoc/>
    public bool IsUnlockLinkUnlocked(uint unlockLink, byte minimumQuestSequence) => this.unlockStateService.IsUnlockLinkUnlocked(unlockLink, minimumQuestSequence);

    /// <inheritdoc/>
    public bool IsUnlockLinkUnlocked(ushort unlockLink) => this.unlockStateService.IsUnlockLinkUnlocked(unlockLink);

    /// <inheritdoc/>
    public bool IsXBMPetUnlocked(XBMPet row) => this.unlockStateService.IsXBMPetUnlocked(row);

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.unlockStateService.Unlock -= this.UnlockForward;
    }

    private void UnlockForward(RowRef rowRef) => this.Unlock?.Invoke(rowRef);
}
