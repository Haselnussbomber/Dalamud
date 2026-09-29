using Dalamud.IoC.Internal;
using Dalamud.Services.Marketboard.Internal;
using Dalamud.Services.Marketboard.Structures;

namespace Dalamud.Services.Marketboard;

/// <summary>
/// This class provides access to market board events.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal sealed class MarketBoard : IInternalDisposableService, IMarketBoard
{
    [ServiceManager.ServiceDependency]
    private readonly NetworkHandlers networkHandlers = Service<NetworkHandlers>.Get();

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketBoard"/> class.
    /// </summary>
    [ServiceManager.ServiceConstructor]
    public MarketBoard()
    {
        this.networkHandlers.MbHistoryObservable.Subscribe(this.OnMbHistory);
        this.networkHandlers.MbPurchaseObservable.Subscribe(this.OnPurchase);
        this.networkHandlers.MbOfferingsObservable.Subscribe(this.OnOfferings);
        this.networkHandlers.MbPurchaseSentObservable.Subscribe(this.OnPurchaseSent);
        this.networkHandlers.MbTaxesObservable.Subscribe(this.OnTaxRates);
    }

    /// <inheritdoc/>
    public event IMarketBoard.HistoryReceivedDelegate? HistoryReceived;

    /// <inheritdoc/>
    public event IMarketBoard.ItemPurchasedDelegate? ItemPurchased;

    /// <inheritdoc/>
    public event IMarketBoard.OfferingsReceivedDelegate? OfferingsReceived;

    /// <inheritdoc/>
    public event IMarketBoard.PurchaseRequestedDelegate? PurchaseRequested;

    /// <inheritdoc/>
    public event IMarketBoard.TaxRatesReceivedDelegate? TaxRatesReceived;

    /// <inheritdoc/>
    public void DisposeService()
    {
        this.HistoryReceived = null;
        this.ItemPurchased = null;
        this.OfferingsReceived = null;
        this.PurchaseRequested = null;
        this.TaxRatesReceived = null;
    }

    private void OnMbHistory(MarketBoardHistory marketBoardHistory)
    {
        this.HistoryReceived?.Invoke(marketBoardHistory);
    }

    private void OnPurchase(MarketBoardPurchase marketBoardHistory)
    {
        this.ItemPurchased?.Invoke(marketBoardHistory);
    }

    private void OnOfferings(MarketBoardCurrentOfferings currentOfferings)
    {
        this.OfferingsReceived?.Invoke(currentOfferings);
    }

    private void OnPurchaseSent(MarketBoardPurchaseHandler purchaseHandler)
    {
        this.PurchaseRequested?.Invoke(purchaseHandler);
    }

    private void OnTaxRates(MarketTaxRates taxRates)
    {
        this.TaxRatesReceived?.Invoke(taxRates);
    }
}
