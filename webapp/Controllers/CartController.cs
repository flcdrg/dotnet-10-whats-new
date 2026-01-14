using Microsoft.AspNetCore.Mvc;
using webapp.Models;
using webapp.Services;
using System.Text.Json;

namespace webapp.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly IGstCalculationService _gstService;
    private readonly IShippingCalculationService _shippingService;
    private readonly ICountryService _countryService;
    private readonly ILogger<CartController> _logger;
    private const string CartSessionKey = "ShoppingCart";

    public CartController(IProductService productService, IGstCalculationService gstService, IShippingCalculationService shippingService, ICountryService countryService, ILogger<CartController> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _gstService = gstService ?? throw new ArgumentNullException(nameof(gstService));
        _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
        _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index()
    {
        var cart = GetCartFromSession();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int petId, int quantity)
    {
        var countryCode = await _countryService.GetCurrentCountryCodeAsync();
        var pet = await _productService.GetProductByIdAsync(petId, countryCode);
        if (pet == null)
        {
            TempData["Error"] = "This pet is not available for your selected country.";
            return RedirectToAction("Index", "Home");
        }

        if (await _productService.IsPetRestrictedAsync(petId, countryCode))
        {
            var countryName = (await _countryService.GetCountryByCodeAsync(countryCode))?.Name ?? countryCode;
            TempData["Error"] = $"{pet.Name} cannot be shipped to {countryName}.";
            return RedirectToAction("Index", "Home");
        }

        var cart = GetCartFromSession();
        var cartItem = new CartItem
        {
            PetId = petId,
            PetName = pet.Name,
            PetPrice = pet.Price,
            Quantity = quantity,
            ImageUrl = pet.ImageUrl
        };

        cart.AddItem(cartItem);
        SaveCartToSession(cart);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int petId)
    {
        var cart = GetCartFromSession();
        cart.RemoveItem(petId);
        SaveCartToSession(cart);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Checkout()
    {
        var cart = GetCartFromSession();
        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index");
        }

        await PopulateCheckoutLookups(await _countryService.GetCurrentCountryCodeAsync());

        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessCheckout(Order order, string country, string state, string shippingMethod)
    {
        var cart = GetCartFromSession();
        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index");
        }

        var selectedCountry = await _countryService.GetCountryByCodeAsync(country);
        if (selectedCountry == null)
        {
            ModelState.AddModelError(nameof(country), "Please select a valid country.");
        }

        if (!country.Equals("Australia", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.Remove(nameof(order.State));
            state = string.Empty;
        }
        else if (string.IsNullOrWhiteSpace(state))
        {
            ModelState.AddModelError(nameof(state), "State is required for Australian addresses.");
        }

        var restrictedItems = new List<string>();
        foreach (var item in cart.Items)
        {
            if (await _productService.IsPetRestrictedAsync(item.PetId, country))
            {
                restrictedItems.Add(item.PetName);
            }
        }

        if (restrictedItems.Count > 0)
        {
            var countryName = selectedCountry?.Name ?? country;
            ModelState.AddModelError(string.Empty, $"The following items cannot be shipped to {countryName}: {string.Join(", ", restrictedItems)}");
        }

        var availableMethods = _shippingService.GetAvailableShippingMethods(country, state);
        if (!availableMethods.Contains(shippingMethod))
        {
            ModelState.AddModelError(nameof(shippingMethod), "Please select a valid shipping method.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateCheckoutLookups(country);
            return View("Checkout", cart);
        }

        await _countryService.SetCurrentCountryAsync(country);

        order.Items = cart.Items;
        order.Subtotal = cart.GetSubtotal();
        order.Country = selectedCountry?.Name ?? country;
        order.State = country.Equals("Australia") ? state : string.Empty;
        order.ShippingMethod = shippingMethod;

        // Calculate shipping (assuming 2kg total weight for now)
        var shippingCost = _shippingService.CalculateShippingCost(country, state, 2m, shippingMethod);
        order.ShippingCost = shippingCost;

        // Calculate GST
        order.GstAmount = _gstService.CalculateGst(order.Subtotal, country);

        // Calculate total
        order.Total = order.Subtotal + order.ShippingCost + order.GstAmount;
        order.OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}";
        order.CreatedAt = DateTime.UtcNow;
        order.LastModifiedAt = DateTime.UtcNow;

        // Clear cart
        cart.ClearCart();
        SaveCartToSession(cart);

        return View("OrderConfirmation", order);
    }

    private ShoppingCart GetCartFromSession()
    {
        var cart = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cart))
        {
            var newCart = new ShoppingCart
            {
                CartId = HttpContext.Session.Id,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };
            return newCart;
        }

        return JsonSerializer.Deserialize<ShoppingCart>(cart) ?? new ShoppingCart();
    }

    private void SaveCartToSession(ShoppingCart cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CartSessionKey, cartJson);
    }

    private async Task PopulateCheckoutLookups(string? selectedCountryCode)
    {
        ViewBag.AvailableCountries = await _countryService.GetAvailableCountriesAsync();
        ViewBag.AustralianStates = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "NT", "ACT" };
        ViewBag.SelectedCountry = string.IsNullOrWhiteSpace(selectedCountryCode)
            ? await _countryService.GetCurrentCountryCodeAsync()
            : selectedCountryCode;
    }
}
