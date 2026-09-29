using Dalamud.Hooking;
using Dalamud.IoC.Internal;
using Dalamud.Logging.Internal;
using Dalamud.Services.PartyFinderGui.Types;

using FFXIVClientStructs.FFXIV.Client.Game.Network;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.Interop;

namespace Dalamud.Services.PartyFinderGui;

/// <summary>
/// This class handles interacting with the native PartyFinder window.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal sealed unsafe class PartyFinderGui : IInternalDisposableService, IPartyFinderGui
{
    private static readonly ModuleLog Log = ModuleLog.Create<PartyFinderGui>();

    private readonly Hook<InfoProxyCrossRealm.Delegates.ReceiveListing> receiveListingHook;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyFinderGui"/> class.
    /// </summary>
    [ServiceManager.ServiceConstructor]
    private PartyFinderGui()
    {
        this.receiveListingHook = Hook<InfoProxyCrossRealm.Delegates.ReceiveListing>.FromAddress(
            InfoProxyCrossRealm.Addresses.ReceiveListing.Value,
            this.HandleReceiveListingDetour);
        this.receiveListingHook.Enable();
    }

    /// <inheritdoc/>
    public event IPartyFinderGui.PartyFinderListingEventDelegate? ReceiveListing;

    /// <summary>
    /// Dispose of managed and unmanaged resources.
    /// </summary>
    void IInternalDisposableService.DisposeService()
    {
        this.receiveListingHook.Dispose();
    }

    private void HandleReceiveListingDetour(InfoProxyCrossRealm* infoProxy, ServerIpcSegment<CrossRealmListingSegmentPacket>* packet)
    {
        try
        {
            this.HandleListingEvents(packet);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception on ReceiveListing hook.");
        }

        this.receiveListingHook.Original(infoProxy, packet);
    }

    private void HandleListingEvents(ServerIpcSegment<CrossRealmListingSegmentPacket>* packet)
    {
        for (var i = 0; i < packet->Payload.Entries.Length; i++)
        {
            var entry = packet->Payload.Entries.GetPointer(i);

            // these are empty slots that are not shown to the player
            if (entry->ListingId == 0)
                continue;

            var listing = new PartyFinderListing(entry);
            var args = new PartyFinderListingEventArgs(packet->Payload.SegmentIndex);
            foreach (var d in Delegate.EnumerateInvocationList(this.ReceiveListing))
            {
                try
                {
                    d(listing, args);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception during raise of {handler}", d.Method);
                }
            }

            if (!args.Visible)
            {
                // hide the listing from the player by setting it to a null listing
                packet->Payload.Entries[i] = default;
            }
        }
    }
}
