using Dalamud.Configuration.Internal;

using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ChatGui;

/// <inheritdoc />
public sealed class PrintableChatMessage : IPrintableChatMessage
{
    /// <inheritdoc />
    public XivChatType LogKind { get; set; } = Service<DalamudConfiguration>.Get().GeneralChatType;

    /// <inheritdoc />
    public XivChatRelationKind SourceKind { get; set; }

    /// <inheritdoc />
    public XivChatRelationKind TargetKind { get; set; }

    /// <inheritdoc />
    public ReadOnlySeString Sender { get; set; }

    /// <inheritdoc />
    public ReadOnlySeString Message { get; set; }

    /// <inheritdoc />
    public int Timestamp { get; set; }

    /// <inheritdoc />
    public bool Silent { get; set; }
}
