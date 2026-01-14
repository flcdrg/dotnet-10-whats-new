using webapp.Models;

namespace webapp.Services;

public interface IProductService
{
    Task<List<Pet>> GetAllProductsAsync(string? countryCode = null);
    Task<Pet?> GetProductByIdAsync(int id, string? countryCode = null);
    Task<List<Pet>> SearchProductsAsync(string searchTerm, string? countryCode = null);
    Task<bool> IsPetRestrictedAsync(int petId, string? countryCode);
}