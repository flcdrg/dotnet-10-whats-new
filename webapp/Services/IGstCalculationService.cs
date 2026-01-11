namespace webapp.Services;

public interface IGstCalculationService
{
    decimal CalculateGst(decimal subtotal, string country);
    bool IsAustralianAddress(string country);
}
