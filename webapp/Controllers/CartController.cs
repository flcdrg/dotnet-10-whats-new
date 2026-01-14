using Microsoft.AspNetCore.Mvc;
using webapp.Application.Commands;
using webapp.Application.Queries;
using webapp.Models;
using Mediator;
using System.Text.Json;

namespace webapp.Controllers;

public class CartController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartController> _logger;
    private const string CartSessionKey = "ShoppingCart";

    public CartController(IMediator mediator, ILogger<CartController> logger)
    {
        _mediator = mediator;
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
        var currentCountry = await _mediator.Send(new GetCurrentCountryQuery());
        var result = await _mediator.Send(new AddToCartCommand
        {
            PetId = petId,
            Quantity = quantity,
            CountryCode = currentCountry
        });

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        var cart = GetCartFromSession();
        cart.AddItem(result.Item!);
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

        var countries = await _mediator.Send(new GetCountriesQuery());
        var selectedCountry = await _mediator.Send(new GetCurrentCountryQuery());

        ViewBag.AvailableCountries = countries;
        ViewBag.AustralianStates = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "NT", "ACT" };
        ViewBag.SelectedCountry = selectedCountry;

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

        var validationResult = await _mediator.Send(new ValidateCheckoutCommand
        {
            Order = order,
            Country = country,
            State = state,
            ShippingMethod = shippingMethod,
            Items = cart.Items
        });

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            var countries = await _mediator.Send(new GetCountriesQuery());
            ViewBag.AvailableCountries = countries;
            ViewBag.AustralianStates = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "NT", "ACT" };
            ViewBag.SelectedCountry = country;

            return View("Checkout", cart);
        }

        var processedOrder = await _mediator.Send(new ProcessCheckoutCommand
        {
            Order = order,
            Items = cart.Items,
            Country = country,
            State = state,
            ShippingMethod = shippingMethod
        });

        // Clear cart
        cart.ClearCart();
        SaveCartToSession(cart);

        return View("OrderConfirmation", processedOrder);
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
