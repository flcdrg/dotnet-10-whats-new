using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapp.Models;
using webapp.Services;

namespace webapp.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IProductService productService, ILogger<HomeController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var pets = await _productService.GetAllProductsAsync();
        return View(pets);
    }

    public async Task<IActionResult> Search(string query)
    {
        var pets = await _productService.SearchProductsAsync(query ?? string.Empty);
        return View("Index", pets);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pet = await _productService.GetProductByIdAsync(id);
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
