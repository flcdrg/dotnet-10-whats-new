using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using webapp.Controllers;
using webapp.Services;
using webapp.Models;
using webapp.Application.Queries;
using Mediator;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Tests.Controllers;

public class HomeControllerTests
{
    private readonly IProductService _productService;
    private readonly IMediator _mediator;
    private readonly ILogger<HomeController> _logger;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _productService = Substitute.For<IProductService>();
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<HomeController>>();
        _controller = new HomeController(_productService, _mediator, _logger);
        SetupControllerContext();
    }

    private void SetupControllerContext()
    {
        var httpContext = new DefaultHttpContext();
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        _controller.TempData = Substitute.For<ITempDataDictionary>();
    }

    #region Index Tests

    [Fact]
    public async Task Index_ReturnsViewWithProductsForCurrentCountry()
    {
        // Arrange
        var pets = new List<Pet> { new() { Id = 1, Name = "Pet1" } };
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));
        _productService.GetAllProductsAsync("AU").Returns(pets);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(pets, viewResult.Model);
    }

    [Fact]
    public async Task Index_GetsCurrentCountryFromMediator()
    {
        // Arrange
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("US"));
        _productService.GetAllProductsAsync("US").Returns(new List<Pet>());

        // Act
        await _controller.Index();

        // Assert
        await _mediator.Received(1).Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Search Tests

    [Fact]
    public async Task Search_WithSearchQuery_ReturnsViewWithSearchResults()
    {
        // Arrange
        var pets = new List<Pet> { new() { Id = 1, Name = "Fluffy" } };
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));
        _productService.SearchProductsAsync("cat", "AU").Returns(pets);

        // Act
        var result = await _controller.Search("cat");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        Assert.Equal(pets, viewResult.Model);
    }

    [Fact]
    public async Task Search_WithNullQuery_PassesEmptyStringToService()
    {
        // Arrange
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));
        _productService.SearchProductsAsync("", "AU").Returns(new List<Pet>());

        // Act
        await _controller.Search("");

        // Assert
        await _productService.Received(1).SearchProductsAsync("", "AU");
    }

    [Fact]
    public async Task Search_WithQuery_CallsServiceWithQuery()
    {
        // Arrange
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("US"));
        _productService.SearchProductsAsync("dog", "US").Returns(new List<Pet>());

        // Act
        await _controller.Search("dog");

        // Assert
        await _productService.Received(1).SearchProductsAsync("dog", "US");
    }

    #endregion

    #region Details Tests

    [Fact]
    public async Task Details_WithAvailablePet_ReturnsViewWithPet()
    {
        // Arrange
        var pet = new Pet { Id = 1, Name = "Fluffy" };
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));
        _productService.IsPetRestrictedAsync(1, "AU").Returns(false);
        _productService.GetProductByIdAsync(1, "AU").Returns(pet);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(pet, viewResult.Model);
    }

    [Fact]
    public async Task Details_WithRestrictedPet_SetsTempDataErrorAndRedirects()
    {
        // Arrange
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("GB"));
        _productService.IsPetRestrictedAsync(2, "GB").Returns(true);

        // Act
        var result = await _controller.Details(2);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.NotNull(_controller.TempData["Error"]);
        Assert.Contains("not available", _controller.TempData["Error"].ToString()!);
    }

    [Fact]
    public async Task Details_WithNonexistentPet_ReturnsNotFound()
    {
        // Arrange
        _mediator.Send(Arg.Any<GetCurrentCountryQuery>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult("AU"));
        _productService.IsPetRestrictedAsync(999, "AU").Returns(false);
        _productService.GetProductByIdAsync(999, "AU").Returns((Pet?)null);

        // Act
        var result = await _controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Privacy Tests

    [Fact]
    public void Privacy_ReturnsView()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    #endregion

    #region Error Tests

    [Fact]
    public void Error_ReturnsViewWithErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model.RequestId);
    }

    #endregion
}
