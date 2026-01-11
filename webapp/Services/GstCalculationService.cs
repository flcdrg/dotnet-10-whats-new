namespace webapp.Services;

public class GstCalculationService : IGstCalculationService
{
    private const decimal AustralianGstRate = 0.10m; // 10%

    public decimal CalculateGst(decimal subtotal, string country)
    {
        if (string.IsNullOrEmpty(country))
        {
            return 0;
        }

        if (IsAustralianAddress(country))
        {
            return subtotal * AustralianGstRate;
        }

        return 0;
    }

    public bool IsAustralianAddress(string country)
    {
        return !string.IsNullOrEmpty(country) && country.Equals("Australia", StringComparison.OrdinalIgnoreCase);
    }
}
