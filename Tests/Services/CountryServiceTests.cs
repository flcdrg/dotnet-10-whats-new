using Xunit;
using NSubstitute;
using webapp.Services;
using Tests.Services.Fixtures;
using Microsoft.AspNetCore.Http;

namespace Tests.Services;

public class CountryServiceTests : IDisposable
{
    private readonly CountryServiceFixture _fixture;
    private readonly CountryService _service;

    public CountryServiceTests()
    {
        _fixture = new CountryServiceFixture();
        _service = new CountryService(_fixture.Context, _fixture.HttpContextAccessor);
    }

    [Fact]
    public async Task GetAvailableCountriesAsync_ReturnsAllCountriesOrderedByName()
    {
        // Act
        var result = await _service.GetAvailableCountriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Australia", result[0].Name);
        Assert.Equal("United Kingdom", result[1].Name);
        Assert.Equal("United States", result[2].Name);
    }

    [Fact]
    public async Task GetCountryByCodeAsync_WithValidCode_ReturnsCountry()
    {
        // Act
        var result = await _service.GetCountryByCodeAsync("AU");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AU", result.Code);
        Assert.Equal("Australia", result.Name);
    }

    [Fact]
    public async Task GetCountryByCodeAsync_WithInvalidCode_ReturnsNull()
    {
        // Act
        var result = await _service.GetCountryByCodeAsync("XX");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCountryByCodeAsync_WithNullCode_ReturnsNull()
    {
        // Act
        var result = await _service.GetCountryByCodeAsync(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCountryByCodeAsync_WithEmptyCode_ReturnsNull()
    {
        // Act
        var result = await _service.GetCountryByCodeAsync("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCountryByCodeAsync_WithWhitespaceCode_ReturnsNull()
    {
        // Act
        var result = await _service.GetCountryByCodeAsync("   ");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentCountryCodeAsync_WithNoHttpContext_ReturnsDefaultCountry()
    {
        // Arrange - Create a service with HTTP context accessor that returns null
        var nullContextAccessor = Substitute.For<IHttpContextAccessor>();
        nullContextAccessor.HttpContext.Returns((HttpContext?)null);
        var serviceNoContext = new CountryService(_fixture.Context, nullContextAccessor);

        // Act
        var result = await serviceNoContext.GetCurrentCountryCodeAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AU", result); // First country in DB
    }

    [Fact]
    public async Task SetCurrentCountryAsync_WithValidCountryCode_Works()
    {
        // Act
        await _service.SetCurrentCountryAsync("US");

        // Assert - Just verify it doesn't throw
        Assert.True(true);
    }

    [Fact]
    public async Task SetCurrentCountryAsync_WithNullCode_Works()
    {
        // Act
        await _service.SetCurrentCountryAsync(null);

        // Assert - Just verify it doesn't throw
        Assert.True(true);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
