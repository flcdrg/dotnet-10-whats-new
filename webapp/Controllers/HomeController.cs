using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapp.Models;
using webapp.Application.Queries;
using webapp.Services;
using Mediator;

namespace webapp.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IMediator _mediator;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IProductService productService, IMediator mediator, ILogger<HomeController> logger)
    {
        _productService = productService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var countryCode = await _mediator.Send(new GetCurrentCountryQuery());
        var pets = await _productService.GetAllProductsAsync(countryCode);
        return View(pets);
    }

    public async Task<IActionResult> Search(string query)
    {
        var countryCode = await _mediator.Send(new GetCurrentCountryQuery());
        var pets = await _productService.SearchProductsAsync(query ?? string.Empty, countryCode);
        return View("Index", pets);
    }

    public async Task<IActionResult> Details(int id)
    {
        var countryCode = await _mediator.Send(new GetCurrentCountryQuery());
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
