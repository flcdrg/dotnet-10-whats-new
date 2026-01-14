using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapp.Models;
using webapp.Services;

namespace webapp.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICountryService _countryService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IProductService productService, ICountryService countryService, ILogger<HomeController> logger)
    {
        _productService = productService;
        _countryService = countryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var countryCode = await _countryService.GetCurrentCountryCodeAsync();
        var pets = await _productService.GetAllProductsAsync(countryCode);
        return View(pets);
    }

    public async Task<IActionResult> Search(string query)
    {
        var countryCode = await _countryService.GetCurrentCountryCodeAsync();
        var pets = await _productService.SearchProductsAsync(query ?? string.Empty, countryCode);
        return View("Index", pets);
    }

    public async Task<IActionResult> Details(int id)
    {
        var countryCode = await _countryService.GetCurrentCountryCodeAsync();
        if (await _productService.IsPetRestrictedAsync(id, countryCode))
        {
            TempData["Error"] = "This pet is not available in your selected country.";
            return RedirectToAction("Index");
        }

        var pet = await _productService.GetProductByIdAsync(id, countryCode);
        if (pet == null)
        {
            return NotFound();
        }
        return View(pet);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
