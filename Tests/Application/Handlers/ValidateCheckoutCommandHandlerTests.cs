using NSubstitute;
using Xunit;
using webapp.Application.Commands;
using webapp.Application.Handlers;
using webapp.Models;
using Tests.Application.Fixtures;

namespace Tests.Application.Handlers;

public class ValidateCheckoutCommandHandlerTests
{
    private readonly CommandHandlerFixture _fixture;
    private readonly ValidateCheckoutCommandHandler _handler;

    public ValidateCheckoutCommandHandlerTests()
    {
        _fixture = new CommandHandlerFixture();
        _handler = new ValidateCheckoutCommandHandler(
            _fixture.CountryService,
            _fixture.ProductService,
            _fixture.ShippingService
        );
    }

    [Fact]
    public async Task Handle_WithValidAustralianCheckout_ReturnsValid()
    {
        // Arrange
        var country = new Country { Code = "AU", Name = "Australia" };
        var items = new List<CartItem> { new() { PetId = 1, PetName = "Pet1" } };

        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        _fixture.ProductService.IsPetRestrictedAsync(1, "AU").Returns(false);
        _fixture.ShippingService.GetAvailableShippingMethods("AU", "NSW").Returns(new List<string> { "AustraliaPost", "Courier" });

        var command = new ValidateCheckoutCommand
        {
            Country = "AU",
            State = "NSW",
            ShippingMethod = "AustraliaPost",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Handle_WithInvalidCountry_ReturnsInvalidWithError()
    {
        // Arrange
        _fixture.CountryService.GetCountryByCodeAsync("XX").Returns((Country?)null);
        var items = new List<CartItem>();

        var command = new ValidateCheckoutCommand
        {
            Country = "XX",
            State = "",
            ShippingMethod = "",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("country"));
        Assert.Contains("valid country", result.Errors["country"]);
    }

    [Fact]
    public async Task Handle_WithAustraliaButNoState_ReturnsInvalidWithStateError()
    {
        // Arrange
        var country = new Country { Code = "AU", Name = "Australia" };
        var items = new List<CartItem>();

        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        _fixture.ShippingService.GetAvailableShippingMethods("AU", "").Returns(new List<string> { "AustraliaPost" });

        var command = new ValidateCheckoutCommand
        {
            Country = "AU",
            State = "",
            ShippingMethod = "AustraliaPost",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("state"));
        Assert.Contains("State is required", result.Errors["state"]);
    }

    [Fact]
    public async Task Handle_WithNonAustraliaState_ClearsState()
    {
        // Arrange
        var country = new Country { Code = "US", Name = "United States" };
        var items = new List<CartItem>();

        _fixture.CountryService.GetCountryByCodeAsync("US").Returns(country);
        _fixture.ShippingService.GetAvailableShippingMethods("US", "").Returns(new List<string> { "International Courier" });

        var command = new ValidateCheckoutCommand
        {
            Country = "US",
            State = "CA",  // State provided but should be cleared for non-AU
            ShippingMethod = "International Courier",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(string.Empty, command.State); // State was cleared
    }

    [Fact]
    public async Task Handle_WithRestrictedItem_ReturnsInvalidWithItemError()
    {
        // Arrange
        var country = new Country { Code = "GB", Name = "United Kingdom" };
        var items = new List<CartItem> { new() { PetId = 2, PetName = "Buddy Dog" } };

        _fixture.CountryService.GetCountryByCodeAsync("GB").Returns(country);
        _fixture.ProductService.IsPetRestrictedAsync(2, "GB").Returns(true);
        _fixture.ShippingService.GetAvailableShippingMethods("GB", "").Returns(new List<string> { "International Courier" });

        var command = new ValidateCheckoutCommand
        {
            Country = "GB",
            State = "",
            ShippingMethod = "International Courier",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("items"));
        Assert.Contains("Buddy Dog", result.Errors["items"]);
        Assert.Contains("United Kingdom", result.Errors["items"]);
    }

    [Fact]
    public async Task Handle_WithMultipleRestrictedItems_ListsAllItems()
    {
        // Arrange
        var country = new Country { Code = "GB", Name = "United Kingdom" };
        var items = new List<CartItem>
        {
            new() { PetId = 2, PetName = "Buddy Dog" },
            new() { PetId = 3, PetName = "Tropical Fish" }
        };

        _fixture.CountryService.GetCountryByCodeAsync("GB").Returns(country);
        _fixture.ProductService.IsPetRestrictedAsync(2, "GB").Returns(true);
        _fixture.ProductService.IsPetRestrictedAsync(3, "GB").Returns(true);
        _fixture.ShippingService.GetAvailableShippingMethods("GB", "").Returns(new List<string> { "International Courier" });

        var command = new ValidateCheckoutCommand
        {
            Country = "GB",
            State = "",
            ShippingMethod = "International Courier",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("items"));
        Assert.Contains("Buddy Dog", result.Errors["items"]);
        Assert.Contains("Tropical Fish", result.Errors["items"]);
    }

    [Fact]
    public async Task Handle_WithInvalidShippingMethod_ReturnsInvalidWithMethodError()
    {
        // Arrange
        var country = new Country { Code = "AU", Name = "Australia" };
        var items = new List<CartItem>();

        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        _fixture.ShippingService.GetAvailableShippingMethods("AU", "NSW").Returns(new List<string> { "AustraliaPost", "Courier" });

        var command = new ValidateCheckoutCommand
        {
            Country = "AU",
            State = "NSW",
            ShippingMethod = "InvalidMethod",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("shippingMethod"));
        Assert.Contains("valid shipping method", result.Errors["shippingMethod"]);
    }

    [Fact]
    public async Task Handle_WithMultipleErrors_AccumulatesAllErrors()
    {
        // Arrange
        var items = new List<CartItem> { new() { PetId = 2, PetName = "Buddy Dog" } };

        _fixture.CountryService.GetCountryByCodeAsync("XX").Returns((Country?)null);
        _fixture.ProductService.IsPetRestrictedAsync(2, "XX").Returns(true);
        _fixture.ShippingService.GetAvailableShippingMethods("XX", "").Returns(new List<string> { "International Courier" });

        var command = new ValidateCheckoutCommand
        {
            Country = "XX",
            State = "",
            ShippingMethod = "InvalidMethod",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("country"));
        Assert.True(result.Errors.ContainsKey("items"));
        Assert.True(result.Errors.ContainsKey("shippingMethod"));
    }

    [Fact]
    public async Task Handle_WithNoItems_ValidatesSuccessfully()
    {
        // Arrange
        var country = new Country { Code = "AU", Name = "Australia" };
        var items = new List<CartItem>();

        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        _fixture.ShippingService.GetAvailableShippingMethods("AU", "NSW").Returns(new List<string> { "AustraliaPost" });

        var command = new ValidateCheckoutCommand
        {
            Country = "AU",
            State = "NSW",
            ShippingMethod = "AustraliaPost",
            Items = items
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
