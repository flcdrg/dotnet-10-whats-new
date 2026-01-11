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
    private readonly ILogger<CartController> _logger;
    private const string CartSessionKey = "ShoppingCart";

    public CartController(IProductService productService, IGstCalculationService gstService, IShippingCalculationService shippingService, ILogger<CartController> logger)
    {
        _productService = productService;
        _gstService = gstService;
        _shippingService = shippingService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var cart = GetCartFromSession();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int petId, int quantity)
    {
        var pet = await _productService.GetProductByIdAsync(petId);
        if (pet == null)
        {
            return NotFound();
        }

        var cart = GetCartFromSession();
        var cartItem = new CartItem();
        cartItem.PetId = petId;
        cartItem.PetName = pet.Name;
        cartItem.PetPrice = pet.Price;
        cartItem.Quantity = quantity;
        cartItem.ImageUrl = pet.ImageUrl;

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

    public IActionResult Checkout()
    {
        var cart = GetCartFromSession();
        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index");
        }

        ViewBag.AvailableCountries = new[] { "Australia", "UK", "NewZealand", "Antarctica" };
        ViewBag.AustralianStates = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "NT", "ACT" };

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessCheckout(Order order, string country, string state, string shippingMethod)
    {
        var cart = GetCartFromSession();
        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index");
        }

        order.Items = cart.Items;
        order.Subtotal = cart.GetSubtotal();
        order.Country = country;
        order.State = state;
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
            var newCart = new ShoppingCart();
            newCart.CartId = HttpContext.Session.Id;
            newCart.CreatedAt = DateTime.UtcNow;
            newCart.LastModifiedAt = DateTime.UtcNow;
            return newCart;
        }

        return JsonSerializer.Deserialize<ShoppingCart>(cart) ?? new ShoppingCart();
    }

    private void SaveCartToSession(ShoppingCart cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CartSessionKey, cartJson);
    }
}
