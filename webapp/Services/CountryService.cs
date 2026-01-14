using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Services;

public interface ICountryService
{
    Task<IReadOnlyList<Country>> GetAvailableCountriesAsync();
    Task<Country?> GetCountryByCodeAsync(string? code);
    Task<string> GetCurrentCountryCodeAsync();
    Task SetCurrentCountryAsync(string? code);
}

public class CountryService : ICountryService
{
    private const string SessionKey = "SelectedCountry";
    private readonly PetstoreContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CountryService(PetstoreContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<Country>> GetAvailableCountriesAsync()
    {
        return await _context.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Country?> GetCountryByCodeAsync(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return await _context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code.Equals(code));
    }

    public async Task<string> GetCurrentCountryCodeAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            return await GetDefaultCountryCodeAsync();
        }

        var sessionValue = context.Session.GetString(SessionKey);
        if (!string.IsNullOrWhiteSpace(sessionValue))
        {
            return sessionValue;
        }

        var defaultCode = await GetDefaultCountryCodeAsync();
        context.Session.SetString(SessionKey, defaultCode);
        return defaultCode;
    }

    public async Task SetCurrentCountryAsync(string? code)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            return;
        }

        var validCountry = await GetCountryByCodeAsync(code) ?? await _context.Countries.AsNoTracking().FirstOrDefaultAsync();
        if (validCountry != null)
        {
            context.Session.SetString(SessionKey, validCountry.Code);
        }
    }

    private async Task<string> GetDefaultCountryCodeAsync()
    {
        var defaultCountry = await _context.Countries
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .FirstOrDefaultAsync();

        return defaultCountry?.Code ?? string.Empty;
    }
}
