using System.Net.Http.Json;
using Demo.AdminWeb.Models;

namespace Demo.AdminWeb.Services;

public class AccessoryApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<AccessoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<List<AccessoryDto>>("/api/accessories", cancellationToken);
        return result ?? new List<AccessoryDto>();
    }

    public async Task<AccessoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<AccessoryDto>($"/api/accessories/{id}", cancellationToken);
    }

    public async Task<(bool Success, string? Error, AccessoryDto? Item)> CreateAsync(AccessoryUpsert request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/accessories", request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var item = await response.Content.ReadFromJsonAsync<AccessoryDto>(cancellationToken: cancellationToken);
            return (true, null, item);
        }

        var error = await ReadErrorAsync(response, cancellationToken);
        return (false, error, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(int id, AccessoryUpsert request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/accessories/{id}", request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var error = await ReadErrorAsync(response, cancellationToken);
        return (false, error);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/api/accessories/{id}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var error = await ReadErrorAsync(response, cancellationToken);
        return (false, error);
    }

    private static async Task<string?> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<Dictionary<string, object?>>(cancellationToken: cancellationToken);
            if (problem is not null && problem.TryGetValue("error", out var value) && value is string s)
            {
                return s;
            }
        }
        catch
        {
            // ignored
        }

        return $"Request failed with status code {(int)response.StatusCode}";
    }
}
