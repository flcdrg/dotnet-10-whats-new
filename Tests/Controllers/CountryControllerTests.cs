using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using webapp.Controllers;
using webapp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Tests.Controllers;

public class CountryControllerTests
{
    private readonly ICountryService _countryService;
    private readonly CountryController _controller;

    public CountryControllerTests()
    {
        _countryService = Substitute.For<ICountryService>();
        _controller = new CountryController(_countryService);
        SetupControllerContext();
    }

    private void SetupControllerContext()
    {
        var httpContext = new DefaultHttpContext();
        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.IsLocalUrl(Arg.Any<string>()).Returns(x =>
        {
            var url = x.Arg<string>();
            return url.StartsWith("/") && !url.StartsWith("//");
        });

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        _controller.Url = urlHelper;
    }

    [Fact]
    public async Task Set_WithValidCountryAndLocalReturnUrl_RedirectsToReturnUrl()
    {
        // Arrange
        const string countryCode = "AU";
        const string returnUrl = "/home/index";

        // Act
        var result = await _controller.Set(countryCode, returnUrl);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal(returnUrl, redirectResult.Url);
        await _countryService.Received(1).SetCurrentCountryAsync(countryCode);
    }

    [Fact]
    public async Task Set_WithValidCountryAndNoReturnUrl_RedirectsToHome()
    {
        // Arrange
        const string countryCode = "US";

        // Act
        var result = await _controller.Set(countryCode, null);

        // Assert
        var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToAction.ActionName);
        Assert.Equal("Home", redirectToAction.ControllerName);
        await _countryService.Received(1).SetCurrentCountryAsync(countryCode);
    }

    [Fact]
    public async Task Set_WithValidCountryAndEmptyReturnUrl_RedirectsToHome()
    {
        // Arrange
        const string countryCode = "GB";
        const string returnUrl = "";

        // Act
        var result = await _controller.Set(countryCode, returnUrl);

        // Assert
        var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToAction.ActionName);
        Assert.Equal("Home", redirectToAction.ControllerName);
    }

    [Fact]
    public async Task Set_WithExternalUrl_RedirectsToHome()
    {
        // Arrange
        const string countryCode = "AU";
        const string returnUrl = "https://external.com/page";

        // Act
        var result = await _controller.Set(countryCode, returnUrl);

        // Assert
        var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToAction.ActionName); // Rejects external URLs
        Assert.Equal("Home", redirectToAction.ControllerName);
    }

    [Fact]
    public async Task Set_WithValidCountry_CallsCountryService()
    {
        // Arrange
        const string countryCode = "AU";

        // Act
        await _controller.Set(countryCode, null);

        // Assert
        await _countryService.Received(1).SetCurrentCountryAsync(countryCode);
    }
}
