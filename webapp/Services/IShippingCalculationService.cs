namespace webapp.Services;

public interface IShippingCalculationService
{
    decimal CalculateShippingCost(string country, string state, decimal weightKg, string shippingMethod);
    List<string> GetAvailableShippingMethods(string country, string state);
}
