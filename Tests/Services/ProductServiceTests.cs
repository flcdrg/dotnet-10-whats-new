using Xunit;
using webapp.Services;
using Tests.Services.Fixtures;

namespace Tests.Services;

public class ProductServiceTests : IDisposable
{
    private readonly ProductServiceFixture _fixture;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _fixture = new ProductServiceFixture();
        _service = new ProductService(_fixture.Context);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithoutCountryCode_ReturnsAllProducts()
    {
        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Buddy Dog", result[0].Name);
        Assert.Equal("Fluffy Cat", result[1].Name);
        Assert.Equal("Tropical Fish", result[2].Name);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithValidCountryCode_ExcludesRestrictedProducts()
    {
        // Act - UK (CountryId 3) has Buddy Dog (id 2) restricted
        var result = await _service.GetAllProductsAsync("GB");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, p => p.Id == 2);
        Assert.Contains(result, p => p.Id == 1);
        Assert.Contains(result, p => p.Id == 3);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithNullCountryCode_ReturnsAllProducts()
    {
        // Act
        var result = await _service.GetAllProductsAsync(null);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Fluffy Cat", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidIdAndValidCountry_ReturnsProduct()
    {
        // Act - Fluffy Cat (id 1) is not restricted for GB
        var result = await _service.GetProductByIdAsync(1, "GB");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Fluffy Cat", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithRestrictedProductAndRestrictedCountry_ReturnsNull()
    {
        // Act - Buddy Dog (id 2) is restricted for GB (CountryId 3)
        var result = await _service.GetProductByIdAsync(2, "GB");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchProductsAsync_WithSearchTerm_FindsMatchingProducts()
    {
        // Act
        var result = await _service.SearchProductsAsync("cat");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Fluffy Cat", result[0].Name);
    }

    [Fact]
    public async Task SearchProductsAsync_WithSearchTermCaseInsensitive_FindsMatches()
    {
        // Act
        var result = await _service.SearchProductsAsync("DOG");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Buddy Dog", result[0].Name);
    }

    [Fact]
    public async Task SearchProductsAsync_WithSearchInDescription_FindsMatches()
    {
        // Act
        var result = await _service.SearchProductsAsync("aquatic");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Tropical Fish", result[0].Name);
    }

    [Fact]
    public async Task SearchProductsAsync_WithSearchInSpecies_FindsMatches()
    {
        // Act
        var result = await _service.SearchProductsAsync("fish");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Tropical Fish", result[0].Name);
    }

    [Fact]
    public async Task SearchProductsAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Act
        var result = await _service.SearchProductsAsync("nonexistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchProductsAsync_WithEmptySearchTerm_ReturnsAllProducts()
    {
        // Act
        var result = await _service.SearchProductsAsync("");

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task SearchProductsAsync_WithNullSearchTerm_ReturnsAllProducts()
    {
        // Act
        var result = await _service.SearchProductsAsync(null!);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task SearchProductsAsync_WithRestrictedCountry_ExcludesRestrictedProducts()
    {
        // Act - Search in GB where Buddy Dog is restricted
        var result = await _service.SearchProductsAsync("", "GB");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, p => p.Id == 2);
    }

    [Fact]
    public async Task IsPetRestrictedAsync_WithRestrictedPet_ReturnsTrue()
    {
        // Act - Buddy Dog (id 2) is restricted for GB (CountryId 3)
        var result = await _service.IsPetRestrictedAsync(2, "GB");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsPetRestrictedAsync_WithUnrestrictedPet_ReturnsFalse()
    {
        // Act - Fluffy Cat (id 1) is not restricted for GB
        var result = await _service.IsPetRestrictedAsync(1, "GB");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsPetRestrictedAsync_WithNullCountryCode_ReturnsFalse()
    {
        // Act
        var result = await _service.IsPetRestrictedAsync(2, null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsPetRestrictedAsync_WithInvalidCountryCode_ReturnsFalse()
    {
        // Act
        var result = await _service.IsPetRestrictedAsync(2, "XX");

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
