using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using webapp.Controllers;
using webapp.Models;
using webapp.Application.Commands;
using webapp.Application.Queries;
using Mediator;
using System.Text.Json;

namespace Tests.Controllers;

public class CartControllerTests
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartController> _logger;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<CartController>>();
        _controller = new CartController(_mediator, _logger);
        SetupControllerContext();
    }

    private void SetupControllerContext()
    {
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        httpContext.Session = session;
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        _controller.TempData = Substitute.For<ITempDataDictionary>();
    }

    #region Index Tests

    [Fact]
    public void Index_ReturnsViewWithCart()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
        Assert.IsType<ShoppingCart>(viewResult.Model);
    }

    #endregion

    #region AddToCart Tests

    [Fact]
    public async Task AddToCart_WithValidPet_AddsToCartAndRedirects()
    {
        // Arrange
        var cartItem = new CartItem { PetId = 1, PetName = "Pet1", Quantity = 1, PetPrice = 50m };
        var addResult = new AddToCartResult { Success = true, Item = cartItem };

        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));
        _mediator.Send(Arg.Any<AddToCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(addResult));

        // Act
        var result = await _controller.AddToCart(1, 1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("CartController", redirectResult.ControllerName ?? "CartController");
    }

    [Fact]
    public async Task AddToCart_WithFailedAdd_SetsTempDataAndRedirectsHome()
    {
        // Arrange
        var addResult = new AddToCartResult { Success = false, ErrorMessage = "Pet not available" };

        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));
        _mediator.Send(Arg.Any<AddToCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(addResult));

        // Act
        var result = await _controller.AddToCart(999, 1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.NotNull(_controller.TempData["Error"]);
    }

    [Fact]
    public async Task AddToCart_GetCurrentCountryFromMediator()
    {
        // Arrange
        var cartItem = new CartItem { PetId = 1, PetName = "Pet1", Quantity = 1, PetPrice = 50m };
        var addResult = new AddToCartResult { Success = true, Item = cartItem };

        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("US"));
        _mediator.Send(Arg.Any<AddToCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(addResult));

        // Act
        await _controller.AddToCart(1, 1);

        // Assert
        await _mediator.Received(1).Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region RemoveFromCart Tests

    [Fact]
    public void RemoveFromCart_RemovesItemAndRedirects()
    {
        // Act
        var result = _controller.RemoveFromCart(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    #endregion

    #region Checkout Tests

    [Fact]
    public async Task Checkout_WithEmptyCart_RedirectsToIndex()
    {
        // Act
        var result = await _controller.Checkout();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Checkout_WithItems_ReturnsCheckoutView()
    {
        // Arrange
        var session = (MockSession)_controller.HttpContext.Session;
        var cart = new ShoppingCart 
        { 
            Items = new List<CartItem> { new() { PetId = 1, PetName = "Pet1" } }
        };
        var cartJson = JsonSerializer.Serialize(cart);
        session.SetString("ShoppingCart", cartJson);
        
        var countries = new List<Country> { new() { Code = "AU", Name = "Australia" } };
        _mediator.Send(Arg.Any<GetCountriesQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult((IReadOnlyList<Country>)countries));
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));

        // Act
        var result = await _controller.Checkout();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public async Task Checkout_SetsViewBagData()
    {
        // Arrange
        var session = (MockSession)_controller.HttpContext.Session;
        var cart = new ShoppingCart 
        { 
            Items = new List<CartItem> { new() { PetId = 1, PetName = "Pet1" } }
        };
        var cartJson = JsonSerializer.Serialize(cart);
        session.SetString("ShoppingCart", cartJson);
        
        var countries = new List<Country> { new() { Code = "AU", Name = "Australia" } };
        _mediator.Send(Arg.Any<GetCountriesQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult((IReadOnlyList<Country>)countries));
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));

        // Act
        await _controller.Checkout();

        // Assert
        Assert.NotNull(_controller.ViewBag.AvailableCountries);
        Assert.NotNull(_controller.ViewBag.AustralianStates);
        Assert.Equal("AU", _controller.ViewBag.SelectedCountry);
    }

    #endregion

    #region ProcessCheckout Tests

    [Fact]
    public async Task ProcessCheckout_WithEmptyCart_RedirectsToIndex()
    {
        // Arrange
        var order = new Order();

        // Act
        var result = await _controller.ProcessCheckout(order, "AU", "NSW", "AustraliaPost");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task ProcessCheckout_WithValidCheckout_CreatesOrderAndRedirects()
    {
        // Arrange
        var session = (MockSession)_controller.HttpContext.Session;
        var cart = new ShoppingCart 
        { 
            Items = new List<CartItem> { new() { PetId = 1, Quantity = 1, PetPrice = 50m } }
        };
        var cartJson = JsonSerializer.Serialize(cart);
        session.SetString("ShoppingCart", cartJson);

        var order = new Order();
        var processedOrder = new Order { OrderNumber = "ORD-123" };

        var validationResult = new ValidateCheckoutResult { IsValid = true };
        _mediator.Send(Arg.Any<ValidateCheckoutCommand>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(validationResult));
        _mediator.Send(Arg.Any<ProcessCheckoutCommand>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(processedOrder));

        // Act
        var result = await _controller.ProcessCheckout(order, "AU", "NSW", "AustraliaPost");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("OrderConfirmation", viewResult.ViewName);
        Assert.Equal(processedOrder, viewResult.Model);
    }

    [Fact]
    public async Task ProcessCheckout_WithValidationErrors_ReturnsCheckoutViewWithErrors()
    {
        // Arrange
        var session = (MockSession)_controller.HttpContext.Session;
        var cart = new ShoppingCart 
        { 
            Items = new List<CartItem> { new() { PetId = 1 } }
        };
        var cartJson = JsonSerializer.Serialize(cart);
        session.SetString("ShoppingCart", cartJson);

        var order = new Order();
        var validationResult = new ValidateCheckoutResult 
        { 
            IsValid = false,
            Errors = new Dictionary<string, string> { { "country", "Invalid country" } }
        };

        var countries = new List<Country> { new() { Code = "AU", Name = "Australia" } };
        _mediator.Send(Arg.Any<ValidateCheckoutCommand>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(validationResult));
        _mediator.Send(Arg.Any<GetCountriesQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult((IReadOnlyList<Country>)countries));

        // Act
        var result = await _controller.ProcessCheckout(order, "XX", "NSW", "AustraliaPost");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Checkout", viewResult.ViewName);
        Assert.False(_controller.ModelState.IsValid);
    }

    #endregion
}

/// <summary>
/// Mock implementation of ISession for testing purposes
/// </summary>
public class MockSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionData = new();

    public string Id { get; } = Guid.NewGuid().ToString();

    public bool IsAvailable => true;

    public IEnumerable<string> Keys => _sessionData.Keys;

    public void Clear()
    {
        _sessionData.Clear();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _sessionData.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        _sessionData[key] = value;
    }

    public bool TryGetValue(string key, out byte[]? value)
    {
        return _sessionData.TryGetValue(key, out value);
    }
}
