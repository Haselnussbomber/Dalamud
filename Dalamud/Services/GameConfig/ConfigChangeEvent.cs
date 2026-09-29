using Dalamud.Utility;

namespace Dalamud.Services.GameConfig;

/// <summary>
/// Represents a change in the configuration.
/// </summary>
[Api16ToDo("Remove ctor(Enum option) and Deconstruct function, which were added to not break the API when Name was added.")]
public abstract record ConfigChangeEvent
{
    /// <summary> Initializes a new instance of the <see cref="ConfigChangeEvent"/> class. </summary>
    /// <param name="option">The option that was changed.</param>
    public ConfigChangeEvent(Enum option)
    {
        this.Option = option;
        this.Name = string.Empty;
    }

    /// <summary> Initializes a new instance of the <see cref="ConfigChangeEvent"/> class. </summary>
    /// <param name="option">The option that was changed.</param>
    /// <param name="name">The name of the option that was changed.</param>
    public ConfigChangeEvent(Enum option, string name)
    {
        this.Option = option;
        this.Name = name;
    }

    /// <summary>
    /// Gets the option that was changed.
    /// </summary>
    public Enum Option { get; init; }

    /// <summary>
    /// Gets the name of the option that was changed.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Deconstructs the <see cref="ConfigChangeEvent"/> record.
    /// </summary>
    /// <param name="option">The option that was changed.</param>
    public void Deconstruct(out Enum option)
    {
        option = this.Option;
    }
}
