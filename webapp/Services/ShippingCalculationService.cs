using webapp.Data;
using Microsoft.EntityFrameworkCore;

namespace webapp.Services;

public class ShippingCalculationService : IShippingCalculationService
{
    private readonly PetstoreContext _context;

    public ShippingCalculationService(PetstoreContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public decimal CalculateShippingCost(string country, string state, decimal weightKg, string shippingMethod)
    {
        if (string.IsNullOrEmpty(country))
        {
            return 0;
        }

        if (IsAustralia(country))
        {
            return CalculateAustralianShipping(state, shippingMethod);
        }

        return CalculateInternationalShipping(country, weightKg);
    }

    private decimal CalculateAustralianShipping(string state, string shippingMethod)
    {
        if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(shippingMethod))
        {
            return 10m; // Default Australia Post rate
        }

        var rate = _context.AustralianShippingRates
            .AsNoTracking()
            .FirstOrDefault(r => 
                r.State == state && 
                r.ShippingMethod == shippingMethod);

        return rate?.Rate ?? 10m;
    }

    private decimal CalculateInternationalShipping(string country, decimal weightKg)
    {
        var rate = _context.InternationalShippingRates
            .AsNoTracking()
            .Include(r => r.Country)
            .FirstOrDefault(r => r.Country != null && r.Country.Code.Equals(country));

        if (rate == null)
        {
            return 0;
        }

        return rate.GetRate(weightKg);
    }

    public List<string> GetAvailableShippingMethods(string country, string state)
    {
        var methods = new List<string>();

        if (IsAustralia(country))
        {
            methods.Add("AustraliaPost");
            methods.Add("Courier");
            return methods;
        }

        var hasInternationalRate = _context.InternationalShippingRates
            .AsNoTracking()
            .Include(r => r.Country)
            .Any(r => r.Country != null && r.Country.Code.Equals(country));

        if (hasInternationalRate)
        {
            methods.Add("International Courier");
        }

        return methods;
    }

    private static bool IsAustralia(string country)
    {
        return country.Equals("Australia");
    }
}
