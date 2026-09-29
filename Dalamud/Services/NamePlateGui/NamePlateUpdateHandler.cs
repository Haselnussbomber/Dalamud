using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using FFXIVClientStructs.Interop;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.NamePlateGui;

/// <summary>
/// A class representing a single nameplate. Provides mechanisms to look up the game object associated with the
/// nameplate and allows for modification of various backing fields in number and string array data, which in turn
/// affect aspects of the nameplate's appearance when drawn. Instances of this class are only valid for a single frame
/// and should not be kept across frames.
/// </summary>
internal unsafe class NamePlateUpdateHandler : INamePlateUpdateHandler
{
    private readonly NamePlateUpdateContext context;

    private ulong? gameObjectId;
    private IGameObject? gameObject;
    private NamePlateInfoView? infoView;
    private NamePlatePartsContainer? partsContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="NamePlateUpdateHandler"/> class.
    /// </summary>
    /// <param name="context">The current update context.</param>
    /// <param name="arrayIndex">The index for this nameplate data in the backing number and string array data. This is
    /// not the same as the rendered index, which can be retrieved from <see cref="NamePlateIndex"/>.</param>
    internal NamePlateUpdateHandler(NamePlateUpdateContext context, int arrayIndex)
    {
        this.context = context;
        this.ArrayIndex = arrayIndex;
    }

    /// <inheritdoc/>
    public int ArrayIndex { get; }

    /// <inheritdoc/>
    public ulong GameObjectId => this.gameObjectId ??= this.NamePlateInfo->ObjectId;

    /// <inheritdoc/>
    public IGameObject? GameObject
    {
        get
        {
            if (this.GameObjectId == 0xE0000000)
            {
                // Skipping Ui3DModule lookup for invalid nameplate (NamePlateInfo->ObjectId is 0xE0000000). This
                // prevents crashes around certain Doman Reconstruction cutscenes.
                return null;
            }

            if (this.ArrayIndex >= this.context.Ui3DModule->NamePlateObjectInfoCount)
                return null;

            var objectInfoPtr = this.context.Ui3DModule->NamePlateObjectInfoPointers[this.ArrayIndex];
            if (objectInfoPtr.Value == null) return null;

            var gameObjectPtr = objectInfoPtr.Value->GameObject;
            if (gameObjectPtr == null) return null;

            return this.gameObject ??= this.context.ObjectTable[gameObjectPtr->ObjectIndex];
        }
    }

    /// <inheritdoc/>
    public IBattleChara? BattleChara => this.GameObject as IBattleChara;

    /// <inheritdoc/>
    public IPlayerCharacter? PlayerCharacter => this.GameObject as IPlayerCharacter;

    /// <inheritdoc/>
    public INamePlateInfoView InfoView => this.infoView ??= new NamePlateInfoView(this.NamePlateInfo);

    /// <inheritdoc/>
    public nint NamePlateInfoAddress => (nint)this.NamePlateInfo;

    /// <inheritdoc/>
    public nint NamePlateObjectAddress => (nint)this.NamePlateObject;

    /// <inheritdoc/>
    public NamePlateKind NamePlateKind => (NamePlateKind)this.ObjectData->NamePlateKind;

    /// <inheritdoc/>
    public int UpdateFlags
    {
        get => this.ObjectData->UpdateFlags;
        private set => this.ObjectData->UpdateFlags = value;
    }

    /// <inheritdoc/>
    public uint TextColor
    {
        get => this.ObjectData->NameTextColor;
        set
        {
            if (value != this.TextColor) this.UpdateFlags |= 2;
            this.ObjectData->NameTextColor = value;
        }
    }

    /// <inheritdoc/>
    public uint EdgeColor
    {
        get => this.ObjectData->NameEdgeColor;
        set
        {
            if (value != this.EdgeColor) this.UpdateFlags |= 2;
            this.ObjectData->NameEdgeColor = value;
        }
    }

    /// <inheritdoc/>
    public int MarkerIconId
    {
        get => this.ObjectData->MarkerIconId;
        set => this.ObjectData->MarkerIconId = value;
    }

    /// <inheritdoc/>
    public int NameIconId
    {
        get => this.ObjectData->NameIconId;
        set => this.ObjectData->NameIconId = value;
    }

    /// <inheritdoc/>
    public int NamePlateIndex => this.ObjectData->NamePlateObjectIndex;

    /// <inheritdoc/>
    public int DrawFlags
    {
        get => this.ObjectData->DrawFlags;
        private set => this.ObjectData->DrawFlags = value;
    }

    /// <inheritdoc/>
    public int VisibilityFlags
    {
        get => this.ObjectData->VisibilityFlags;
        set => this.ObjectData->VisibilityFlags = value;
    }

    /// <inheritdoc/>
    public bool IsUpdating
    {
        get => (this.UpdateFlags & 1) != 0;
        internal set => this.UpdateFlags = value ? this.UpdateFlags | 1 : this.UpdateFlags & ~1;
    }

    /// <inheritdoc/>
    public bool IsPrefixTitle
    {
        get => (this.DrawFlags & 1) != 0;
        set => this.DrawFlags = value ? this.DrawFlags | 1 : this.DrawFlags & ~1;
    }

    /// <inheritdoc/>
    public bool DisplayTitle
    {
        get => (this.DrawFlags & 0x80) == 0;
        set => this.DrawFlags = value ? this.DrawFlags & ~0x80 : this.DrawFlags | 0x80;
    }

    /// <inheritdoc/>
    public ReadOnlySeString Name
    {
        get => this.GetFieldAsReadOnlySeString(NamePlateStringField.Name);
        set => this.WeakSetField(NamePlateStringField.Name, value);
    }

    /// <inheritdoc/>
    public NamePlateSimpleParts NameParts => this.PartsContainer.Name;

    /// <inheritdoc/>
    public ReadOnlySeString Title
    {
        get => this.GetFieldAsReadOnlySeString(NamePlateStringField.Title);
        set => this.WeakSetField(NamePlateStringField.Title, value);
    }

    /// <inheritdoc/>
    public NamePlateQuotedParts TitleParts => this.PartsContainer.Title;

    /// <inheritdoc/>
    public ReadOnlySeString FreeCompanyTag
    {
        get => this.GetFieldAsReadOnlySeString(NamePlateStringField.FreeCompanyTag);
        set => this.WeakSetField(NamePlateStringField.FreeCompanyTag, value);
    }

    /// <inheritdoc/>
    public NamePlateQuotedParts FreeCompanyTagParts => this.PartsContainer.FreeCompanyTag;

    /// <inheritdoc/>
    public ReadOnlySeString StatusPrefix
    {
        get => this.GetFieldAsReadOnlySeString(NamePlateStringField.StatusPrefix);
        set => this.WeakSetField(NamePlateStringField.StatusPrefix, value);
    }

    /// <inheritdoc/>
    public ReadOnlySeString TargetSuffix
    {
        get => this.GetFieldAsReadOnlySeString(NamePlateStringField.TargetSuffix);
        set => this.WeakSetField(NamePlateStringField.TargetSuffix, value);
    }

    /// <inheritdoc/>
    public ReadOnlySeString LevelPrefix
    {
        get => this.GetFieldAsReadOnlySeString(NamePlateStringField.LevelPrefix);
        set => this.WeakSetField(NamePlateStringField.LevelPrefix, value);
    }

    /// <summary>
    /// Gets or (lazily) creates a part builder container for this nameplate.
    /// </summary>
    internal NamePlatePartsContainer PartsContainer =>
        this.partsContainer ??= new NamePlatePartsContainer(this.context);

    private RaptureAtkModule.NamePlateInfo* NamePlateInfo =>
        this.context.RaptureAtkModule->NamePlateInfoEntries.GetPointer(this.NamePlateIndex);

    private AddonNamePlate.NamePlateObject* NamePlateObject =>
        &this.context.Addon->NamePlateObjectArray[this.NamePlateIndex];

    private NamePlateNumberArray.NamePlateObjectIntArrayData* ObjectData =>
        this.context.NumberStruct->ObjectData.GetPointer(this.ArrayIndex);

    /// <inheritdoc/>
    public void RemoveName() => this.RemoveField(NamePlateStringField.Name);

    /// <inheritdoc/>
    public void RemoveTitle() => this.RemoveField(NamePlateStringField.Title);

    /// <inheritdoc/>
    public void RemoveFreeCompanyTag() => this.RemoveField(NamePlateStringField.FreeCompanyTag);

    /// <inheritdoc/>
    public void RemoveStatusPrefix() => this.RemoveField(NamePlateStringField.StatusPrefix);

    /// <inheritdoc/>
    public void RemoveTargetSuffix() => this.RemoveField(NamePlateStringField.TargetSuffix);

    /// <inheritdoc/>
    public void RemoveLevelPrefix() => this.RemoveField(NamePlateStringField.LevelPrefix);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* GetFieldAsPointer(NamePlateStringField field)
    {
        return this.context.StringData->StringArray[this.ArrayIndex + (int)field];
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> GetFieldAsSpan(NamePlateStringField field)
    {
        return MemoryMarshal.CreateReadOnlySpanFromNullTerminated(this.GetFieldAsPointer(field));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetFieldAsString(NamePlateStringField field)
    {
        return Encoding.UTF8.GetString(this.GetFieldAsSpan(field));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySeString GetFieldAsReadOnlySeString(NamePlateStringField field)
    {
        return new ReadOnlySeString(this.GetFieldAsSpan(field));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySeStringSpan GetFieldAsReadOnlySeStringSpan(NamePlateStringField field)
    {
        return this.GetFieldAsSpan(field);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(NamePlateStringField field, string value)
    {
        this.context.StringData->SetValue(this.ArrayIndex + (int)field, value, true, true, true);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(NamePlateStringField field, ReadOnlySeString value)
    {
        using var rssb = new RentedSeStringBuilder();
        this.context.StringData->SetValue(
            this.ArrayIndex + (int)field,
            rssb.Builder.Append(value).GetViewAsSpan(),
            true,
            true,
            true);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(NamePlateStringField field, ReadOnlySeStringSpan value)
    {
        using var rssb = new RentedSeStringBuilder();
        this.context.StringData->SetValue(
            this.ArrayIndex + (int)field,
            rssb.Builder.Append(value).GetViewAsSpan(),
            true,
            true,
            true);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(NamePlateStringField field, byte* value)
    {
        this.context.StringData->SetValue(this.ArrayIndex + (int)field, value, true, true, true);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveField(NamePlateStringField field)
    {
        this.context.StringData->SetValue(
            this.ArrayIndex + (int)field,
            (byte*)NamePlateGui.EmptyStringPointer,
            true,
            false,
            true);
    }

    /// <summary>
    /// Resets the state of this handler for re-use in a new update.
    /// </summary>
    internal void ResetState()
    {
        this.gameObjectId = null;
        this.gameObject = null;
        this.infoView = null;
        this.partsContainer = null;
    }

    /// <summary>
    /// Sets the string array value for the provided field, unless it was already set to the special empty string
    /// pointer used by the Remove methods.
    /// </summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The ReadOnlySeString to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WeakSetField(NamePlateStringField field, ReadOnlySeString value)
    {
        if ((nint)this.GetFieldAsPointer(field) == NamePlateGui.EmptyStringPointer)
            return;

        using var rssb = new RentedSeStringBuilder();
        this.context.StringData->SetValue(
            this.ArrayIndex + (int)field,
            rssb.Builder.Append(value).GetViewAsSpan(),
            true,
            true,
            true);
    }
}
