using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Dalamud.Services.DataManager;
using Dalamud.Services.SeStringEvaluator;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.Text;
using FFXIVClientStructs.Interop;

using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace Dalamud.Services.ChatGui;

/// <summary>
/// This class represents log message in the queue to be added to the chat.
/// </summary>
internal sealed unsafe class LogMessage : ILogMessage
{
    /// <summary>
    /// Gets a shared instance of this class.
    /// </summary>
    public static LogMessage Instance { get; } = new();

    /// <summary>
    /// Gets or sets the native message wrapped by this object.
    /// </summary>
    public LogMessageQueueItem* Pointer { get; set; }

    /// <inheritdoc/>
    public nint Address => (nint)this.Pointer;

    /// <inheritdoc/>
    public uint LogMessageId => this.Pointer->LogMessageId;

    /// <inheritdoc/>
    public RowRef<Lumina.Excel.Sheets.LogMessage> GameData => LuminaUtils.CreateRef<Lumina.Excel.Sheets.LogMessage>(this.Pointer->LogMessageId);

    /// <inheritdoc/>
    ILogMessageEntity? ILogMessage.SourceEntity => this.Pointer->SourceKind == EntityRelationKind.None ? null : this.SourceEntity;

    /// <inheritdoc/>
    ILogMessageEntity? ILogMessage.TargetEntity => this.Pointer->TargetKind == EntityRelationKind.None ? null : this.TargetEntity;

    /// <inheritdoc/>
    public int ParameterCount => this.Pointer->Parameters.Count;

    /// <inheritdoc/>
    public IReadOnlyList<SeStringParameter> Parameters => LogMessageParameterList.Instance;

    /// <inheritdoc/>
    public bool IsHandled { get; set; }

    private LogMessageEntity SourceEntity => new(this.Pointer, true);

    private LogMessageEntity TargetEntity => new(this.Pointer, false);

    public static bool operator ==(LogMessage x, LogMessage y) => x.Equals(y);

    public static bool operator !=(LogMessage x, LogMessage y) => !(x == y);

    /// <inheritdoc/>
    public bool Equals(ILogMessage? other)
    {
        return other is LogMessage logMessage && this.Equals(logMessage);
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is LogMessage logMessage && this.Equals(logMessage);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.LogMessageId, this.SourceEntity, this.TargetEntity);
    }

    /// <inheritdoc/>
    public void PreventOriginal()
    {
        this.IsHandled = true;
    }

    /// <inheritdoc/>
    public bool TryGetIntParameter(int index, out int value)
    {
        value = 0;
        if (!this.TryGetParameter(index, out var parameter)) return false;
        if (parameter.Type != TextParameterType.Integer) return false;
        value = parameter.IntValue;
        return true;
    }

    /// <inheritdoc/>
    public bool TryGetStringParameter(int index, out ReadOnlySeString value)
    {
        value = default;
        if (!this.TryGetParameter(index, out var parameter)) return false;
        if (parameter.Type == TextParameterType.String)
        {
            value = new(parameter.StringValue.AsSpan());
            return true;
        }

        if (parameter.Type == TextParameterType.ReferencedUtf8String)
        {
            value = new(parameter.ReferencedUtf8StringValue->Utf8String.AsSpan());
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public ReadOnlySeString FormatLogMessageForDebugging()
    {
        var logModule = RaptureLogModule.Instance();

        // the formatting logic is taken from RaptureLogModule_Update

        using var utf8 = new Utf8String();
        SetName(logModule, this.SourceEntity);
        SetName(logModule, this.TargetEntity);

        using var rssb = new RentedSeStringBuilder();
        logModule->RaptureTextModule->FormatString(rssb.Builder.Append(this.GameData.Value.Text).GetViewAsSpan(), &this.Pointer->Parameters, &utf8);

        return new ReadOnlySeString(utf8.AsSpan());

        static void SetName(RaptureLogModule* self, LogMessageEntity item)
        {
            var name = item.NameSpan.GetPointer(0);

            if (item.IsPlayer)
            {
                var str = self->TempParseMessage.GetPointer(item.IsSourceEntity ? 8 : 9);
                self->FormatPlayerLink(name, str, null, 0, item.Kind != 1 /* LocalPlayer */, item.HomeWorldId, false, null, false);

                if (item.HomeWorldId != 0 && item.HomeWorldId != AgentLobby.Instance()->LobbyData.HomeWorldId)
                {
                    var crossWorldSymbol = self->RaptureTextModule->UnkStrings0.GetPointer(3);
                    if (!crossWorldSymbol->StringPtr.HasValue)
                        self->RaptureTextModule->ProcessMacroCode(crossWorldSymbol, "<icon(88)>\0"u8);
                    str->Append(crossWorldSymbol);
                    if (self->UIModule->GetWorldHelper()->AllWorlds.TryGetValuePointer(item.HomeWorldId, out var world))
                        str->ConcatCStr(world->Name);
                }

                name = str->StringPtr;
            }

            if (item.IsSourceEntity)
            {
                self->RaptureTextModule->SetGlobalTempEntity1(name, item.Sex, item.ObjStrId);
            }
            else
            {
                self->RaptureTextModule->SetGlobalTempEntity2(name, item.Sex, item.ObjStrId);
            }
        }
    }

    private bool TryGetParameter(int index, out TextParameter value)
    {
        if (index < 0 || index >= this.Pointer->Parameters.Count)
        {
            value = default;
            return false;
        }

        value = this.Pointer->Parameters[index];
        return true;
    }

    private bool Equals(LogMessage other)
    {
        return this.LogMessageId == other.LogMessageId && this.SourceEntity == other.SourceEntity && this.TargetEntity == other.TargetEntity;
    }
}
