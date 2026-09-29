using System.Collections;
using System.Collections.Generic;

using Dalamud.Services.SeStringEvaluator;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Component.Text;
using FFXIVClientStructs.STD;

namespace Dalamud.Services.ChatGui;

/// <summary>
/// This struct represents log message in the queue to be added to the chat.
/// </summary>
internal unsafe class LogMessageParameterList : IReadOnlyList<SeStringParameter>
{
    /// <summary>
    /// Gets a shared instance of this class.
    /// </summary>
    public static LogMessageParameterList Instance { get; } = new();

    /// <summary>
    /// Gets or sets the native list wrapped by this object.
    /// </summary>
    public StdDeque<TextParameter>* Pointer { get; set; }

    /// <inheritdoc />
    public int Count => this.Pointer->Count;

    /// <inheritdoc />
    public SeStringParameter this[int index]
    {
        get
        {
            var p = (*this.Pointer)[index];

            if (p.Type == TextParameterType.Uninitialized)
                return default;
            if (p.Type == TextParameterType.Integer)
                return new((uint)p.IntValue);
            if (p.Type == TextParameterType.String)
                return new(p.StringValue.AsReadOnlySeString());
            if (p.Type == TextParameterType.ReferencedUtf8String)
                return new(p.ReferencedUtf8StringValue->Utf8String.AsReadOnlySeString());

            throw new InvalidOperationException($"Invalid parameter type {p.Type}");
        }
    }

    /// <inheritdoc />
    public IEnumerator<SeStringParameter> GetEnumerator()
    {
        for (var i = 0; i < this.Count; i++)
        {
            yield return this[i];
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
