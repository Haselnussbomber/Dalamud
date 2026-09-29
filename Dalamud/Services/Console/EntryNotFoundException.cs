namespace Dalamud.Services.Console;

/// <summary>
/// Exception thrown when a console entry is not found.
/// </summary>
public sealed class EntryNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntryNotFoundException"/> class.
    /// </summary>
    /// <param name="name">The name of the entry.</param>
    public EntryNotFoundException(string name)
        : base($"Console entry '{name}' does not exist.")
    {
    }
}
