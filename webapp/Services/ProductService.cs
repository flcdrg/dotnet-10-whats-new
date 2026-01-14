using webapp.Data;
using webapp.Models;
using Microsoft.EntityFrameworkCore;

namespace webapp.Services;

public class ProductService(PetstoreContext context) : IProductService
{
    public async Task<List<Pet>> GetAllProductsAsync(string? countryCode = null)
    {
        var query = context.Pets.AsNoTracking().AsQueryable();
        query = await ApplyCountryRestrictionsAsync(query, countryCode);
        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Pet?> GetProductByIdAsync(int id, string? countryCode = null)
    {
        var query = context.Pets.AsNoTracking().Where(p => p.Id == id);
        query = await ApplyCountryRestrictionsAsync(query, countryCode);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<Pet>> SearchProductsAsync(string searchTerm, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllProductsAsync(countryCode);
        }

        var lowerSearchTerm = searchTerm.ToLower();
        var query = context.Pets
            .AsNoTracking()
            .Where(p => p.Name.ToLower().Contains(lowerSearchTerm) ||
                        p.Description.ToLower().Contains(lowerSearchTerm) ||
                        p.Species.ToLower().Contains(lowerSearchTerm));

        query = await ApplyCountryRestrictionsAsync(query, countryCode);

        return await query
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<bool> IsPetRestrictedAsync(int petId, string? countryCode)
    {
        var countryId = await GetCountryIdByCodeAsync(countryCode);
        if (!countryId.HasValue)
        {
            return false;
        }

        return await context.PetShippingRestrictions
            .AsNoTracking()
            .AnyAsync(r => r.PetId == petId && r.CountryId == countryId.Value);
    }

    private async Task<IQueryable<Pet>> ApplyCountryRestrictionsAsync(IQueryable<Pet> query, string? countryCode)
    {
        var countryId = await GetCountryIdByCodeAsync(countryCode);
        if (!countryId.HasValue)
        {
            return query;
        }

        return query.Where(p => !context.PetShippingRestrictions.Any(r => r.PetId == p.Id && r.CountryId == countryId.Value));
    }

    private async Task<int?> GetCountryIdByCodeAsync(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return null;
        }

        var country = await context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code.Equals(countryCode));

        return country?.Id;
    }
}
