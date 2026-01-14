using NSubstitute;
using Xunit;
using webapp.Application.Queries;
using webapp.Models;
using Tests.Application.Fixtures;

namespace Tests.Application.Queries;

public class QueryHandlerTests
{
    private readonly CommandHandlerFixture _fixture;

    public QueryHandlerTests()
    {
        _fixture = new CommandHandlerFixture();
    }

    #region GetPetsQuery Tests

    [Fact]
    public async Task GetPetsQueryHandler_DelegatesToProductService()
    {
        // Arrange
        var pets = new List<Pet> { new() { Id = 1, Name = "Pet1" } };
        _fixture.ProductService.GetAllProductsAsync().Returns(pets);
        var handler = new GetPetsQueryHandler(_fixture.ProductService);
        var query = new GetPetsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(pets, result);
        await _fixture.ProductService.Received(1).GetAllProductsAsync();
    }

    #endregion

    #region SearchPetsQuery Tests

    [Fact]
    public async Task SearchPetsQueryHandler_PassesSearchTermToService()
    {
        // Arrange
        var pets = new List<Pet> { new() { Id = 1, Name = "Fluffy" } };
        _fixture.ProductService.SearchProductsAsync("cat").Returns(pets);
        var handler = new SearchPetsQueryHandler(_fixture.ProductService);
        var query = new SearchPetsQuery("cat");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(pets, result);
        await _fixture.ProductService.Received(1).SearchProductsAsync("cat");
    }

    [Fact]
    public async Task SearchPetsQueryHandler_WithEmptySearchTerm_PassesEmptyTerm()
    {
        // Arrange
        var pets = new List<Pet>();
        _fixture.ProductService.SearchProductsAsync("").Returns(pets);
        var handler = new SearchPetsQueryHandler(_fixture.ProductService);
        var query = new SearchPetsQuery("");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        await _fixture.ProductService.Received(1).SearchProductsAsync("");
    }

    #endregion

    #region GetCountriesQuery Tests

    [Fact]
    public async Task GetCountriesQueryHandler_DelegatesToCountryService()
    {
        // Arrange
        var countries = new List<Country> 
        { 
            new() { Code = "AU", Name = "Australia" },
            new() { Code = "US", Name = "United States" }
        };
        _fixture.CountryService.GetAvailableCountriesAsync().Returns((IReadOnlyList<Country>)countries);
        var handler = new GetCountriesQueryHandler(_fixture.CountryService);
        var query = new GetCountriesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Australia", result[0].Name);
        await _fixture.CountryService.Received(1).GetAvailableCountriesAsync();
    }

    #endregion

    #region GetCurrentCountryQuery Tests

    [Fact]
    public async Task GetCurrentCountryQueryHandler_DelegatesToCountryService()
    {
        // Arrange
        _fixture.CountryService.GetCurrentCountryCodeAsync().Returns("AU");
        var handler = new GetCurrentCountryQueryHandler(_fixture.CountryService);
        var query = new GetCurrentCountryQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("AU", result);
        await _fixture.CountryService.Received(1).GetCurrentCountryCodeAsync();
    }

    #endregion

    #region GetCountryByCodeQuery Tests

    [Fact]
    public async Task GetCountryByCodeQueryHandler_PassesCodeToService()
    {
        // Arrange
        var country = new Country { Code = "AU", Name = "Australia" };
        _fixture.CountryService.GetCountryByCodeAsync("AU").Returns(country);
        var handler = new GetCountryByCodeQueryHandler(_fixture.CountryService);
        var query = new GetCountryByCodeQuery("AU");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("Australia", result!.Name);
        await _fixture.CountryService.Received(1).GetCountryByCodeAsync("AU");
    }

    [Fact]
    public async Task GetCountryByCodeQueryHandler_WithNullCode_PassesNullToService()
    {
        // Arrange
        _fixture.CountryService.GetCountryByCodeAsync(null).Returns((Country?)null);
        var handler = new GetCountryByCodeQueryHandler(_fixture.CountryService);
        var query = new GetCountryByCodeQuery(null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        await _fixture.CountryService.Received(1).GetCountryByCodeAsync(null);
    }

    #endregion

    #region GetPetDetailsQuery Tests

    [Fact]
    public async Task GetPetDetailsQueryHandler_PassesIdToService()
    {
        // Arrange
        var pet = new Pet { Id = 1, Name = "Fluffy" };
        _fixture.ProductService.GetProductByIdAsync(1, null).Returns(pet);
        var handler = new GetPetDetailsQueryHandler(_fixture.ProductService);
        var query = new GetPetDetailsQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("Fluffy", result!.Name);
        await _fixture.ProductService.Received(1).GetProductByIdAsync(1, null);
    }

    [Fact]
    public async Task GetPetDetailsQueryHandler_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _fixture.ProductService.GetProductByIdAsync(999, null).Returns((Pet?)null);
        var handler = new GetPetDetailsQueryHandler(_fixture.ProductService);
        var query = new GetPetDetailsQuery(999);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region CalculateShippingQuery Tests

    [Fact]
    public async Task CalculateShippingQueryHandler_PassesParametersToService()
    {
        // Arrange
        _fixture.ShippingService.CalculateShippingCost("AU", "NSW", 2m, "AustraliaPost").Returns(12.50m);
        var handler = new CalculateShippingQueryHandler(_fixture.ShippingService);
        var query = new CalculateShippingQuery("AU", "NSW", 2m, "AustraliaPost");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(12.50m, result);
        _fixture.ShippingService.Received(1).CalculateShippingCost("AU", "NSW", 2m, "AustraliaPost");
    }

    [Fact]
    public async Task CalculateShippingQueryHandler_WithInternationalShipping_ReturnsCorrectCost()
    {
        // Arrange
        _fixture.ShippingService.CalculateShippingCost("US", "", 5m, "International Courier").Returns(25m);
        var handler = new CalculateShippingQueryHandler(_fixture.ShippingService);
        var query = new CalculateShippingQuery("US", "", 5m, "International Courier");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(25m, result);
    }

    #endregion
}
