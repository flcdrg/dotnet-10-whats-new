using NSubstitute;
using Xunit;
using webapp.Application.Commands;
using webapp.Application.Handlers;
using webapp.Models;
using Tests.Application.Fixtures;

namespace Tests.Application.Handlers;

public class ProcessCheckoutCommandHandlerTests
{
    private readonly CommandHandlerFixture _fixture;
    private readonly ProcessCheckoutCommandHandler _handler;

    public ProcessCheckoutCommandHandlerTests()
    {
        _fixture = new CommandHandlerFixture();
        _handler = new ProcessCheckoutCommandHandler(
            _fixture.CountryService,
            _fixture.GstService,
            _fixture.ShippingService,
            _fixture.TimeProvider);
    }

    [Fact]
    public async Task Handle_WithValidCheckout_CreatesOrderWithCorrectTotals()
    {
        // Arrange
        var now = new DateTimeOffset(2025, 1, 14, 12, 0, 0, TimeSpan.Zero);
        var country = new Country { Code = "AU", Name = "Australia" };
        var items = new List<CartItem>
        {
            new() { PetId = 1, Quantity = 2, PetPrice = 50m },
            new() { PetId = 2, Quantity = 1, PetPrice = 30m }
        };

        _fixture.TimeProvider.SetUtcNow(now);
        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        _fixture.ShippingService.CalculateShippingCost("AU", "NSW", 2m, "AustraliaPost").Returns(12.50m);
        _fixture.GstService.CalculateGst(130m, "AU").Returns(13m);

        var order = new Order();
        var command = new ProcessCheckoutCommand
        {
            Order = order,
            Items = items,
            Country = "AU",
            State = "NSW",
            ShippingMethod = "AustraliaPost"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(items, result.Items);
        Assert.Equal(130m, result.Subtotal); // (50*2) + (30*1)
        Assert.Equal("Australia", result.Country);
        Assert.Equal("NSW", result.State);
        Assert.Equal("AustraliaPost", result.ShippingMethod);
        Assert.Equal(12.50m, result.ShippingCost);
        Assert.Equal(13m, result.GstAmount);
        Assert.Equal(155.50m, result.Total); // 130 + 12.50 + 13
        Assert.StartsWith("ORD-", result.OrderNumber);
        Assert.Equal(now.UtcDateTime, result.CreatedAt);
        Assert.Equal(now.UtcDateTime, result.LastModifiedAt);
    }

    [Fact]
    public async Task Handle_WithNonAustralianCountry_ClearsState()
    {
        // Arrange
        var now = new DateTimeOffset(2025, 1, 14, 12, 0, 0, TimeSpan.Zero);
        var country = new Country { Code = "US", Name = "United States" };
        var items = new List<CartItem> { new() { PetId = 1, Quantity = 1, PetPrice = 100m } };

        _fixture.TimeProvider.SetUtcNow(now);
        _fixture.CountryService.GetCountryByCodeAsync("US").Returns(country);
        _fixture.ShippingService.CalculateShippingCost("US", "", 2m, "International Courier").Returns(25m);
        _fixture.GstService.CalculateGst(100m, "US").Returns(0m);

        var order = new Order();
        var command = new ProcessCheckoutCommand
        {
            Order = order,
            Items = items,
            Country = "US",
            State = "CA",
            ShippingMethod = "International Courier"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(string.Empty, result.State);
    }

    [Fact]
    public async Task Handle_WithUnknownCountry_UsesCountryCode()
    {
        // Arrange
        var now = new DateTimeOffset(2025, 1, 14, 12, 0, 0, TimeSpan.Zero);
        var items = new List<CartItem> { new() { PetId = 1, Quantity = 1, PetPrice = 100m } };

        _fixture.TimeProvider.SetUtcNow(now);
        _fixture.CountryService.GetCountryByCodeAsync("XX").Returns((Country?)null);
        _fixture.ShippingService.CalculateShippingCost("XX", "", 2m, "Method").Returns(0m);
        _fixture.GstService.CalculateGst(100m, "XX").Returns(0m);

        var order = new Order();
        var command = new ProcessCheckoutCommand
        {
            Order = order,
            Items = items,
            Country = "XX",
            State = "",
            ShippingMethod = "Method"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("XX", result.Country); // Uses code as fallback
    }

    [Fact]
    public async Task Handle_SetsOrderNumber()
    {
        // Arrange
        var now = new DateTimeOffset(2025, 1, 14, 12, 0, 0, TimeSpan.Zero);
        var country = new Country { Code = "AU", Name = "Australia" };
        var items = new List<CartItem>();

        _fixture.TimeProvider.SetUtcNow(now);
        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        _fixture.ShippingService.CalculateShippingCost("AU", "NSW", 2m, "AustraliaPost").Returns(10m);
        _fixture.GstService.CalculateGst(0m, "AU").Returns(0m);

        var order = new Order();
        var command = new ProcessCheckoutCommand
        {
            Order = order,
            Items = items,
            Country = "AU",
            State = "NSW",
            ShippingMethod = "AustraliaPost"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var expectedOrderNumber = $"ORD-{now.UtcTicks}";
        Assert.Equal(expectedOrderNumber, result.OrderNumber);
    }

    [Fact]
    public async Task Handle_UpdatesCurrentCountry()
    {
        // Arrange
        var now = new DateTimeOffset(2025, 1, 14, 12, 0, 0, TimeSpan.Zero);
        var country = new Country { Code = "US", Name = "United States" };
        var items = new List<CartItem>();

        _fixture.TimeProvider.SetUtcNow(now);
        _fixture.CountryService.GetCountryByCodeAsync("US").Returns(country);
        _fixture.ShippingService.CalculateShippingCost("US", "", 2m, "International Courier").Returns(25m);
        _fixture.GstService.CalculateGst(0m, "US").Returns(0m);

        var order = new Order();
        var command = new ProcessCheckoutCommand
        {
            Order = order,
            Items = items,
            Country = "US",
            State = "",
            ShippingMethod = "International Courier"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _fixture.CountryService.Received(1).SetCurrentCountryAsync("US");
    }

    [Fact]
    public async Task Handle_WithMultipleItems_CalculatesCorrectSubtotal()
    {
        // Arrange
        var now = new DateTimeOffset(2025, 1, 14, 12, 0, 0, TimeSpan.Zero);
        var country = new Country { Code = "AU", Name = "Australia" };
        var items = new List<CartItem>
        {
            new() { PetId = 1, Quantity = 3, PetPrice = 25.50m },
            new() { PetId = 2, Quantity = 2, PetPrice = 100m }
        };

        _fixture.TimeProvider.SetUtcNow(now);
        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        _fixture.ShippingService.CalculateShippingCost("AU", "NSW", 2m, "AustraliaPost").Returns(10m);
        _fixture.GstService.CalculateGst(276.50m, "AU").Returns(27.65m);

        var order = new Order();
        var command = new ProcessCheckoutCommand
        {
            Order = order,
            Items = items,
            Country = "AU",
            State = "NSW",
            ShippingMethod = "AustraliaPost"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // (25.50 * 3) + (100 * 2) = 76.50 + 200 = 276.50
        Assert.Equal(276.50m, result.Subtotal);
        Assert.Equal(314.15m, result.Total);
    }
}
