using Dalamud.Utility;

namespace Dalamud.Services.GameConfig;

/// <summary>
/// Represents a generic change in the configuration.
/// </summary>
/// <typeparam name="T">The type of the option.</typeparam>
[Api16ToDo("Remove ctor(T option) and Deconstruct function, which were added to not break the API when Name was added.")]
public record ConfigChangeEvent<T> : ConfigChangeEvent where T : Enum
{
    /// <summary> Initializes a new instance of the <see cref="ConfigChangeEvent{T}"/> class. </summary>
    /// <param name="option">The option that was changed.</param>
    public ConfigChangeEvent(T option)
        : base(option)
    {
        this.ConfigOption = option;
    }

    /// <summary> Initializes a new instance of the <see cref="ConfigChangeEvent{T}"/> class. </summary>
    /// <param name="option">The option that was changed.</param>
    /// <param name="name">The name of the option that was changed.</param>
    public ConfigChangeEvent(T option, string name)
        : base(option, name)
    {
        this.ConfigOption = option;
    }

    /// <summary>
    /// Gets the option that was changed.
    /// </summary>
    public T ConfigOption { get; init; }

    /// <summary>
    /// Deconstructs the <see cref="ConfigChangeEvent{T}"/> record.
    /// </summary>
    /// <param name="configOption">The option that was changed.</param>
    public void Deconstruct(out T configOption)
    {
        configOption = this.ConfigOption;
    }
}
