using Mediator;
using webapp.Application.Commands;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Handlers;

public class ProcessCheckoutCommandHandler : IRequestHandler<ProcessCheckoutCommand, Order>
{
    private readonly ICountryService _countryService;
    private readonly IGstCalculationService _gstService;
    private readonly IShippingCalculationService _shippingService;
    private readonly TimeProvider _timeProvider;

    public ProcessCheckoutCommandHandler(
        ICountryService countryService,
        IGstCalculationService gstService,
        IShippingCalculationService shippingService,
        TimeProvider timeProvider)
    {
        _countryService = countryService;
        _gstService = gstService;
        _shippingService = shippingService;
        _timeProvider = timeProvider;
    }

    public async ValueTask<Order> Handle(ProcessCheckoutCommand request, CancellationToken cancellationToken)
    {
        var selectedCountry = await _countryService.GetCountryByCodeAsync(request.Country);

        var order = request.Order;
        order.Items = request.Items;
        order.Subtotal = request.Items.Sum(i => i.GetLineTotal());
        order.Country = selectedCountry?.Name ?? request.Country;
        order.State = request.Country.Equals("AU", StringComparison.OrdinalIgnoreCase) ? request.State : string.Empty;
        order.ShippingMethod = request.ShippingMethod;

        // Calculate shipping (assuming 2kg total weight for now)
        var shippingCost = _shippingService.CalculateShippingCost(request.Country, request.State, 2m, request.ShippingMethod);
        order.ShippingCost = shippingCost;

        // Calculate GST
        order.GstAmount = _gstService.CalculateGst(order.Subtotal, request.Country);

        // Calculate total
        order.Total = order.Subtotal + order.ShippingCost + order.GstAmount;
        var now = _timeProvider.GetUtcNow();
        var utcNow = now.UtcDateTime;
        order.OrderNumber = $"ORD-{now.UtcTicks}";
        order.CreatedAt = utcNow;
        order.LastModifiedAt = utcNow;

        await _countryService.SetCurrentCountryAsync(request.Country);

        return order;
    }
}
