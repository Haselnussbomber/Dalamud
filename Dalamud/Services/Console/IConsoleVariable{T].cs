namespace Dalamud.Services.Console;

/// <summary>
/// Interface representing a variable in the console.
/// </summary>
/// <typeparam name="T">The type of the variable.</typeparam>
public interface IConsoleVariable<T> : IConsoleEntry
{
    /// <summary>
    /// Gets or sets the value of this variable.
    /// </summary>
    T Value { get; set; }
}
