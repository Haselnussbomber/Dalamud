using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.NamePlateGui;

/// <summary>
/// A class representing a single nameplate. Provides mechanisms to look up the game object associated with the
/// nameplate and allows for modification of various backing fields in number and string array data, which in turn
/// affect aspects of the nameplate's appearance when drawn. Instances of this class are only valid for a single frame
/// and should not be kept across frames.
/// </summary>
public interface INamePlateUpdateHandler
{
    /// <summary>
    /// Gets the GameObjectId of the game object associated with this nameplate.
    /// </summary>
    ulong GameObjectId { get; }

    /// <summary>
    /// Gets the <see cref="IGameObject"/> associated with this nameplate, if possible. Performs an object table scan
    /// and caches the result if successful.
    /// </summary>
    IGameObject? GameObject { get; }

    /// <summary>
    /// Gets a read-only view of the nameplate info object data for a nameplate. Modifications to
    /// <see cref="NamePlateUpdateHandler"/> fields do not affect fields in the returned view.
    /// </summary>
    INamePlateInfoView InfoView { get; }

    /// <summary>
    /// Gets the index for this nameplate data in the backing number and string array data. This is not the same as the
    /// rendered or object index, which can be retrieved from <see cref="NamePlateIndex"/>.
    /// </summary>
    int ArrayIndex { get; }

    /// <summary>
    /// Gets the <see cref="IBattleChara"/> associated with this nameplate, if possible. Returns null if the nameplate
    /// has an associated <see cref="IGameObject"/>, but that object cannot be assigned to <see cref="IBattleChara"/>.
    /// </summary>
    IBattleChara? BattleChara { get; }

    /// <summary>
    /// Gets the <see cref="IPlayerCharacter"/> associated with this nameplate, if possible. Returns null if the
    /// nameplate has an associated <see cref="IGameObject"/>, but that object cannot be assigned to
    /// <see cref="IPlayerCharacter"/>.
    /// </summary>
    IPlayerCharacter? PlayerCharacter { get; }

    /// <summary>
    /// Gets the address of the nameplate info struct.
    /// </summary>
    nint NamePlateInfoAddress { get; }

    /// <summary>
    /// Gets the address of the first entry associated with this nameplate in the NamePlate addon's int array.
    /// </summary>
    nint NamePlateObjectAddress { get; }

    /// <summary>
    /// Gets a value indicating what kind of nameplate this is, based on the kind of object it is associated with.
    /// </summary>
    NamePlateKind NamePlateKind { get; }

    /// <summary>
    /// Gets the update flags for this nameplate.
    /// </summary>
    int UpdateFlags { get; }

    /// <summary>
    /// Gets or sets the overall text color for this nameplate. If this value is changed, the appropriate update flag
    /// will be set so that the game will reflect this change immediately.
    /// </summary>
    uint TextColor { get; set; }

    /// <summary>
    /// Gets or sets the overall text edge color for this nameplate. If this value is changed, the appropriate update
    /// flag will be set so that the game will reflect this change immediately.
    /// </summary>
    uint EdgeColor { get; set; }

    /// <summary>
    /// Gets or sets the icon ID for the nameplate's marker icon, which is the large icon used to indicate quest
    /// availability and so on. This value is read from and reset by the game every frame, not just when a nameplate
    /// changes. Setting this to 0 disables the icon.
    /// </summary>
    int MarkerIconId { get; set; }

    /// <summary>
    /// Gets or sets the icon ID for the nameplate's name icon, which is the small icon shown to the left of the name.
    /// Setting this to -1 disables the icon.
    /// </summary>
    int NameIconId { get; set; }

    /// <summary>
    /// Gets the nameplate index, which is the index used for rendering and looking up entries in the object array. For
    /// number and string array data, <see cref="ArrayIndex"/> is used.
    /// </summary>
    int NamePlateIndex { get; }

    /// <summary>
    /// Gets the draw flags for this nameplate.
    /// </summary>
    int DrawFlags { get; }

    /// <summary>
    /// Gets or sets the visibility flags for this nameplate.
    /// </summary>
    int VisibilityFlags { get; set; }

    /// <summary>
    /// Gets a value indicating whether this nameplate is undergoing a major update or not. This is usually true when a
    /// nameplate has just appeared or something meaningful about the entity has changed (e.g. its job or status). This
    /// flag is reset by the game during the update process (during requested update and before draw).
    /// </summary>
    bool IsUpdating { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the title (when visible) will be displayed above the object's name (a
    /// prefix title) instead of below the object's name (a suffix title).
    /// </summary>
    bool IsPrefixTitle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the title should be displayed at all.
    /// </summary>
    bool DisplayTitle { get; set; }

    /// <summary>
    /// Gets or sets the name for this nameplate.
    /// </summary>
    ReadOnlySeString Name { get; set; }

    /// <summary>
    /// Gets a builder which can be used to help cooperatively build a new name for this nameplate even when other
    /// plugins modifying the name are present. Specifically, this builder allows setting text and text-wrapping
    /// payloads (e.g. for setting text color) separately.
    /// </summary>
    NamePlateSimpleParts NameParts { get; }

    /// <summary>
    /// Gets or sets the title for this nameplate.
    /// </summary>
    ReadOnlySeString Title { get; set; }

    /// <summary>
    /// Gets a builder which can be used to help cooperatively build a new title for this nameplate even when other
    /// plugins modifying the title are present. Specifically, this builder allows setting text, text-wrapping
    /// payloads (e.g. for setting text color), and opening and closing quote sequences separately.
    /// </summary>
    NamePlateQuotedParts TitleParts { get; }

    /// <summary>
    /// Gets or sets the free company tag for this nameplate.
    /// </summary>
    ReadOnlySeString FreeCompanyTag { get; set; }

    /// <summary>
    /// Gets a builder which can be used to help cooperatively build a new FC tag for this nameplate even when other
    /// plugins modifying the FC tag are present. Specifically, this builder allows setting text, text-wrapping
    /// payloads (e.g. for setting text color), and opening and closing quote sequences separately.
    /// </summary>
    NamePlateQuotedParts FreeCompanyTagParts { get; }

    /// <summary>
    /// Gets or sets the status prefix for this nameplate. This prefix is used by the game to add BitmapFontIcon-based
    /// online status icons to player nameplates.
    /// </summary>
    ReadOnlySeString StatusPrefix { get; set; }

    /// <summary>
    /// Gets or sets the target suffix for this nameplate. This suffix is used by the game to add the squared-letter
    /// target tags to the end of combat target nameplates.
    /// </summary>
    ReadOnlySeString TargetSuffix { get; set; }

    /// <summary>
    /// Gets or sets the level prefix for this nameplate. This "Lv60" style prefix is added to enemy and friendly battle
    /// NPC nameplates to indicate the NPC level.
    /// </summary>
    ReadOnlySeString LevelPrefix { get; set; }

    /// <summary>
    /// Removes the contents of the name field for this nameplate. This differs from simply setting the field
    /// to an empty string because it writes a special value to memory, and other setters (except SetField variants)
    /// will refuse to overwrite this value. Therefore, fields removed this way are more likely to stay removed.
    /// </summary>
    void RemoveName();

    /// <summary>
    /// Removes the contents of the title field for this nameplate. This differs from simply setting the field
    /// to an empty string because it writes a special value to memory, and other setters (except SetField variants)
    /// will refuse to overwrite this value. Therefore, fields removed this way are more likely to stay removed.
    /// </summary>
    void RemoveTitle();

    /// <summary>
    /// Removes the contents of the FC tag field for this nameplate. This differs from simply setting the field
    /// to an empty string because it writes a special value to memory, and other setters (except SetField variants)
    /// will refuse to overwrite this value. Therefore, fields removed this way are more likely to stay removed.
    /// </summary>
    void RemoveFreeCompanyTag();

    /// <summary>
    /// Removes the contents of the status prefix field for this nameplate. This differs from simply setting the field
    /// to an empty string because it writes a special value to memory, and other setters (except SetField variants)
    /// will refuse to overwrite this value. Therefore, fields removed this way are more likely to stay removed.
    /// </summary>
    void RemoveStatusPrefix();

    /// <summary>
    /// Removes the contents of the target suffix field for this nameplate. This differs from simply setting the field
    /// to an empty string because it writes a special value to memory, and other setters (except SetField variants)
    /// will refuse to overwrite this value. Therefore, fields removed this way are more likely to stay removed.
    /// </summary>
    void RemoveTargetSuffix();

    /// <summary>
    /// Removes the contents of the level prefix field for this nameplate. This differs from simply setting the field
    /// to an empty string because it writes a special value to memory, and other setters (except SetField variants)
    /// will refuse to overwrite this value. Therefore, fields removed this way are more likely to stay removed.
    /// </summary>
    void RemoveLevelPrefix();

    /// <summary>
    /// Gets a pointer to the string array value in the provided field.
    /// </summary>
    /// <param name="field">The field to read from.</param>
    /// <returns>A pointer to a sequence of non-null bytes.</returns>
    unsafe byte* GetFieldAsPointer(NamePlateStringField field);

    /// <summary>
    /// Gets a byte span containing the string array value in the provided field.
    /// </summary>
    /// <param name="field">The field to read from.</param>
    /// <returns>A ReadOnlySpan containing a sequence of non-null bytes.</returns>
    ReadOnlySpan<byte> GetFieldAsSpan(NamePlateStringField field);

    /// <summary>
    /// Gets a UTF8 string copy of the string array value in the provided field.
    /// </summary>
    /// <param name="field">The field to read from.</param>
    /// <returns>A copy of the string array value as a string.</returns>
    string GetFieldAsString(NamePlateStringField field);

    /// <summary>
    /// Gets a ReadOnlySeString copy of the string array value in the provided field.
    /// </summary>
    /// <param name="field">The field to read from.</param>
    /// <returns>A copy of the string array value as a ReadOnlySeString.</returns>
    ReadOnlySeString GetFieldAsReadOnlySeString(NamePlateStringField field);

    /// <summary>
    /// Gets a ReadOnlySeStringSpan view of the string array value in the provided field.
    /// </summary>
    /// <param name="field">The field to read from.</param>
    /// <returns>A copy of the string array value as a ReadOnlySeStringSpan.</returns>
    ReadOnlySeStringSpan GetFieldAsReadOnlySeStringSpan(NamePlateStringField field);

    /// <summary>
    /// Sets the string array value for the provided field.
    /// </summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The string to write.</param>
    void SetField(NamePlateStringField field, string value);

    /// <summary>
    /// Sets the string array value for the provided field.
    /// </summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The ReadOnlySeString to write.</param>
    void SetField(NamePlateStringField field, ReadOnlySeString value);

    /// <summary>
    /// Sets the string array value for the provided field.
    /// </summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The ReadOnlySeStringSpan to write.</param>
    void SetField(NamePlateStringField field, ReadOnlySeStringSpan value);

    /// <summary>
    /// Sets the string array value for the provided field. The provided byte sequence must be null-terminated.
    /// </summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The pointer to a null-terminated sequence of bytes to write.</param>
    unsafe void SetField(NamePlateStringField field, byte* value);

    /// <summary>
    /// Sets the string array value for the provided field to a fixed pointer to an empty string in unmanaged memory.
    /// Other methods may notice this fixed pointer and refuse to overwrite it, preserving the emptiness of the field.
    /// </summary>
    /// <param name="field">The field to write to.</param>
    void RemoveField(NamePlateStringField field);
}
