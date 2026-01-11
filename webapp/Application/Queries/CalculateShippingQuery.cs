using Mediator;
using webapp.Services;

namespace webapp.Application.Queries;

public class CalculateShippingQuery : IRequest<decimal>
{
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string ShippingMethod { get; set; } = string.Empty;

    public CalculateShippingQuery(string country, string state, decimal weightKg, string shippingMethod)
    {
        Country = country;
        State = state;
        WeightKg = weightKg;
        ShippingMethod = shippingMethod;
    }
}

public class CalculateShippingQueryHandler : IRequestHandler<CalculateShippingQuery, decimal>
{
    private readonly IShippingCalculationService _shippingService;

    public CalculateShippingQueryHandler(IShippingCalculationService shippingService)
    {
        _shippingService = shippingService;
    }

    public async ValueTask<decimal> Handle(CalculateShippingQuery request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _shippingService.CalculateShippingCost(request.Country, request.State, request.WeightKg, request.ShippingMethod);
    }
}
