using NSubstitute;
using Xunit;
using webapp.Application.Commands;
using webapp.Models;
using Tests.Application.Fixtures;

namespace Tests.Application.Handlers;

public class AddToCartCommandHandlerTests
{
    private readonly CommandHandlerFixture _fixture;
    private readonly AddToCartCommandHandler _handler;

    public AddToCartCommandHandlerTests()
    {
        _fixture = new CommandHandlerFixture();
        _handler = new AddToCartCommandHandler(_fixture.ProductService, _fixture.CountryService);
    }

    [Fact]
    public async Task Handle_WithValidPet_ReturnsSuccessResult()
    {
        // Arrange
        var pet = new Pet { Id = 1, Name = "Fluffy", Price = 50m, ImageUrl = "fluffy.jpg" };
        _fixture.ProductService.GetProductByIdAsync(1, "AU").Returns(pet);
        _fixture.ProductService.IsPetRestrictedAsync(1, "AU").Returns(false);

        var command = new AddToCartCommand { PetId = 1, Quantity = 2, CountryCode = "AU" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Item);
        Assert.Equal(1, result.Item.PetId);
        Assert.Equal("Fluffy", result.Item.PetName);
        Assert.Equal(50m, result.Item.PetPrice);
        Assert.Equal(2, result.Item.Quantity);
        Assert.Equal("fluffy.jpg", result.Item.ImageUrl);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WithUnavailablePet_ReturnsFailureResult()
    {
        // Arrange
        _fixture.ProductService.GetProductByIdAsync(999, "AU").Returns((Pet?)null);
        var command = new AddToCartCommand { PetId = 999, Quantity = 1, CountryCode = "AU" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Item);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("not available", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WithRestrictedPet_ReturnsFailureWithCountryName()
    {
        // Arrange
        var pet = new Pet { Id = 2, Name = "Buddy Dog", Price = 300m, ImageUrl = "buddy.jpg" };
        var country = new Country { Code = "GB", Name = "United Kingdom" };
        
        _fixture.ProductService.GetProductByIdAsync(2, "GB").Returns(pet);
        _fixture.ProductService.IsPetRestrictedAsync(2, "GB").Returns(true);
        _fixture.CountryService.GetCountryByCodeAsync("GB").Returns(country);

        var command = new AddToCartCommand { PetId = 2, Quantity = 1, CountryCode = "GB" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Item);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Buddy Dog", result.ErrorMessage);
        Assert.Contains("United Kingdom", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WithRestrictedPetAndInvalidCountry_ReturnsFailureWithCountryCode()
    {
        // Arrange
        var pet = new Pet { Id = 2, Name = "Buddy Dog", Price = 300m, ImageUrl = "buddy.jpg" };
        
        _fixture.ProductService.GetProductByIdAsync(2, "XX").Returns(pet);
        _fixture.ProductService.IsPetRestrictedAsync(2, "XX").Returns(true);
        _fixture.CountryService.GetCountryByCodeAsync("XX").Returns((Country?)null);

        var command = new AddToCartCommand { PetId = 2, Quantity = 1, CountryCode = "XX" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("XX", result.ErrorMessage); // Falls back to country code
    }

    [Fact]
    public async Task Handle_WithMultipleQuantity_ReturnsCorrectQuantity()
    {
        // Arrange
        var pet = new Pet { Id = 1, Name = "Fluffy", Price = 50m, ImageUrl = "fluffy.jpg" };
        _fixture.ProductService.GetProductByIdAsync(1, "AU").Returns(pet);
        _fixture.ProductService.IsPetRestrictedAsync(1, "AU").Returns(false);

        var command = new AddToCartCommand { PetId = 1, Quantity = 5, CountryCode = "AU" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(5, result.Item!.Quantity);
    }
}
