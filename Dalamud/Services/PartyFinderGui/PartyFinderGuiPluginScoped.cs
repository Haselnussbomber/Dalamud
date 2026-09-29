using Dalamud.IoC;
using Dalamud.IoC.Internal;
using Dalamud.Services.PartyFinderGui.Types;

namespace Dalamud.Services.PartyFinderGui;

/// <summary>
/// A scoped variant of the PartyFinderGui service.
/// </summary>
[PluginInterface]
[ServiceManager.ScopedService]
#pragma warning disable SA1015
[ResolveVia<IPartyFinderGui>]
#pragma warning restore SA1015
internal sealed class PartyFinderGuiPluginScoped : IInternalDisposableService, IPartyFinderGui
{
    [ServiceManager.ServiceDependency]
    private readonly PartyFinderGui partyFinderGuiService = Service<PartyFinderGui>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyFinderGuiPluginScoped"/> class.
    /// </summary>
    internal PartyFinderGuiPluginScoped()
    {
        this.partyFinderGuiService.ReceiveListing += this.ReceiveListingForward;
    }

    /// <inheritdoc/>
    public event IPartyFinderGui.PartyFinderListingEventDelegate? ReceiveListing;

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.partyFinderGuiService.ReceiveListing -= this.ReceiveListingForward;

        this.ReceiveListing = null;
    }

    private void ReceiveListingForward(IPartyFinderListing listing, IPartyFinderListingEventArgs args) => this.ReceiveListing?.Invoke(listing, args);
}
