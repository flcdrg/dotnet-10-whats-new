using webapp.Data;
using webapp.Models;
using Microsoft.EntityFrameworkCore;

namespace webapp.Services;

public class ShippingCalculationService : IShippingCalculationService
{
    private readonly PetstoreContext _context;

    public ShippingCalculationService(PetstoreContext context)
    {
        _context = context;
    }

    public decimal CalculateShippingCost(string country, string state, decimal weightKg, string shippingMethod)
    {
        if (string.IsNullOrEmpty(country))
        {
            return 0;
        }

        if (country.Equals("Australia", StringComparison.OrdinalIgnoreCase))
        {
            return CalculateAustralianShipping(state, shippingMethod);
        }
        else
        {
            return CalculateInternationalShipping(country, weightKg);
        }
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
                r.State.Equals(state, StringComparison.OrdinalIgnoreCase) && 
                r.ShippingMethod.Equals(shippingMethod, StringComparison.OrdinalIgnoreCase));

        return rate?.Rate ?? 10m;
    }

    private decimal CalculateInternationalShipping(string country, decimal weightKg)
    {
        var rate = _context.InternationalShippingRates
            .AsNoTracking()
            .FirstOrDefault(r => r.Country.Equals(country, StringComparison.OrdinalIgnoreCase));

        if (rate == null)
        {
            return 0;
        }

        return rate.GetRate(weightKg);
    }

    public List<string> GetAvailableShippingMethods(string country, string state)
    {
        var methods = new List<string>();

        if (country.Equals("Australia", StringComparison.OrdinalIgnoreCase))
        {
            methods.Add("AustraliaPost");
            methods.Add("Courier");
        }
        else if (country.Equals("UK", StringComparison.OrdinalIgnoreCase) ||
                 country.Equals("NewZealand", StringComparison.OrdinalIgnoreCase) ||
                 country.Equals("Antarctica", StringComparison.OrdinalIgnoreCase))
        {
            methods.Add("International Courier");
        }

        return methods;
    }
}
