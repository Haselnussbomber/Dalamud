using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Dalamud.Interface.Textures;
using Dalamud.Services.KeyState;

namespace Dalamud.Services.TitleScreenMenu;

/// <summary>
/// Class representing an entry in the title screen menu.
/// </summary>
public class TitleScreenMenuEntry : ITitleScreenMenuEntry
{
    private readonly Action onTriggered;

    /// <summary>
    /// Initializes a new instance of the <see cref="TitleScreenMenuEntry"/> class.
    /// </summary>
    /// <param name="callingAssembly">The calling assembly.</param>
    /// <param name="priority">The priority of this entry.</param>
    /// <param name="text">The text to show.</param>
    /// <param name="texture">The texture to show.</param>
    /// <param name="onTriggered">The action to execute when the option is selected.</param>
    /// <param name="showConditionKeys">The keys that have to be held to display the menu.</param>
    internal TitleScreenMenuEntry(
        Assembly? callingAssembly,
        ulong priority,
        string text,
        ISharedImmediateTexture texture,
        Action onTriggered,
        IEnumerable<VirtualKey>? showConditionKeys = null)
    {
        this.CallingAssembly = callingAssembly;
        this.Priority = priority;
        this.Name = text;
        this.Texture = texture;
        this.onTriggered = onTriggered;
        this.ShowConditionKeys = (showConditionKeys ?? Array.Empty<VirtualKey>()).ToImmutableSortedSet();
    }

    /// <inheritdoc/>
    public ulong Priority { get; init; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public ISharedImmediateTexture Texture { get; set; }

    /// <inheritdoc/>
    public bool IsInternal { get; set; }

    /// <inheritdoc/>
    public Assembly? CallingAssembly { get; init; }

    /// <inheritdoc/>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <inheritdoc/>
    public IReadOnlySet<VirtualKey> ShowConditionKeys { get; init; }

    /// <inheritdoc/>
    public int CompareTo(TitleScreenMenuEntry? other)
    {
        if (other == null)
            return 1;
        if (this.CallingAssembly != other.CallingAssembly)
        {
            if (this.CallingAssembly == null && other.CallingAssembly == null)
                return 0;
            if (this.CallingAssembly == null && other.CallingAssembly != null)
                return -1;
            if (this.CallingAssembly != null && other.CallingAssembly == null)
                return 1;
            return string.Compare(
                this.CallingAssembly!.FullName!,
                other.CallingAssembly!.FullName!,
                StringComparison.CurrentCultureIgnoreCase);
        }

        if (this.Priority != other.Priority)
            return this.Priority.CompareTo(other.Priority);
        if (this.Name != other.Name)
            return string.Compare(this.Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        return 0;
    }

    /// <summary>
    /// Determines the displaying condition of this menu entry is met.
    /// </summary>
    /// <returns>True if met.</returns>
    public bool IsShowConditionSatisfied() =>
        this.ShowConditionKeys.All(x => Service<KeyState.KeyState>.GetNullable()?[x] is true);

    /// <summary>
    /// Trigger the action associated with this entry.
    /// </summary>
    public void Trigger()
    {
        this.onTriggered();
    }
}
