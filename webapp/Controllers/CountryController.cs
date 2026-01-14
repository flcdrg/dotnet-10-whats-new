using Microsoft.AspNetCore.Mvc;
using webapp.Services;

namespace webapp.Controllers;

[AutoValidateAntiforgeryToken]
public class CountryController : Controller
{
    private readonly ICountryService _countryService;

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpPost]
    public async Task<IActionResult> Set(string countryCode, string? returnUrl)
    {
        await _countryService.SetCurrentCountryAsync(countryCode);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
