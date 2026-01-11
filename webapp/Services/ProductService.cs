using webapp.Data;
using webapp.Models;
using Microsoft.EntityFrameworkCore;

namespace webapp.Services;

public interface IProductService
{
    Task<List<Pet>> GetAllProductsAsync();
    Task<Pet?> GetProductByIdAsync(int id);
    Task<List<Pet>> SearchProductsAsync(string searchTerm);
}

public class ProductService : IProductService
{
    private readonly PetstoreContext _context;

    public ProductService(PetstoreContext context)
    {
        _context = context;
    }

    public async Task<List<Pet>> GetAllProductsAsync()
    {
        return await _context.Pets.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Pet?> GetProductByIdAsync(int id)
    {
        return await _context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Pet>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllProductsAsync();
        }

        var lowerSearchTerm = searchTerm.ToLower();
        return await _context.Pets
            .AsNoTracking()
            .Where(p => p.Name.ToLower().Contains(lowerSearchTerm) || 
                        p.Description.ToLower().Contains(lowerSearchTerm) ||
                        p.Species.ToLower().Contains(lowerSearchTerm))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
