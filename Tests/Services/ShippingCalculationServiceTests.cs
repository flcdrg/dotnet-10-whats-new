using Xunit;
using webapp.Services;
using Tests.Services.Fixtures;

namespace Tests.Services;

public class ShippingCalculationServiceTests : IDisposable
{
    private readonly ShippingCalculationServiceFixture _fixture;
    private readonly ShippingCalculationService _service;

    public ShippingCalculationServiceTests()
    {
        _fixture = new ShippingCalculationServiceFixture();
        _service = new ShippingCalculationService(_fixture.Context);
    }

    #region Australian Shipping Tests

    [Fact]
    public void CalculateShippingCost_ForAustraliaNSWAustraliaPost_ReturnsCorrectRate()
    {
        // Act
        var result = _service.CalculateShippingCost("Australia", "NSW", 2m, "AustraliaPost");

        // Assert
        Assert.Equal(12.50m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForAustraliaVICCourier_ReturnsCorrectRate()
    {
        // Act
        var result = _service.CalculateShippingCost("Australia", "VIC", 2m, "Courier");

        // Assert
        Assert.Equal(22.50m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForAustraliaQLDAustraliaPost_ReturnsCorrectRate()
    {
        // Act
        var result = _service.CalculateShippingCost("Australia", "QLD", 2m, "AustraliaPost");

        // Assert
        Assert.Equal(15.50m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForAustraliaWithoutRate_ReturnsDefaultRate()
    {
        // Act - Rate doesn't exist for this combination
        var result = _service.CalculateShippingCost("Australia", "TAS", 2m, "UnknownMethod");

        // Assert
        Assert.Equal(10m, result); // Default rate
    }

    [Fact]
    public void CalculateShippingCost_ForAustraliaWithEmptyState_ReturnsDefaultRate()
    {
        // Act
        var result = _service.CalculateShippingCost("Australia", "", 2m, "AustraliaPost");

        // Assert
        Assert.Equal(10m, result); // Default rate
    }

    [Fact]
    public void CalculateShippingCost_ForAustraliaWithEmptyMethod_ReturnsDefaultRate()
    {
        // Act
        var result = _service.CalculateShippingCost("Australia", "NSW", 2m, "");

        // Assert
        Assert.Equal(10m, result); // Default rate
    }

    #endregion

    #region International Shipping Tests

    [Fact]
    public void CalculateShippingCost_ForUSUnder10kg_ReturnsBaseCost()
    {
        // Act - US has base cost 25m for max 10kg
        var result = _service.CalculateShippingCost("US", "", 5m, "");

        // Assert
        Assert.Equal(25m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForUSAt10kg_ReturnsBaseCost()
    {
        // Act - Exactly at max weight
        var result = _service.CalculateShippingCost("US", "", 10m, "");

        // Assert
        Assert.Equal(25m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForUSOver10kg_ChargesExtraWeight()
    {
        // Act - US: base 25m for 10kg, extra 2.5m per kg over
        // 15kg = 25 + (5 * 2.5) = 37.50
        var result = _service.CalculateShippingCost("US", "", 15m, "");

        // Assert
        Assert.Equal(37.50m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForGBUnder10kg_ReturnsBaseCost()
    {
        // Act - GB has base cost 20m for max 10kg
        var result = _service.CalculateShippingCost("GB", "", 8m, "");

        // Assert
        Assert.Equal(20m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForGBOver10kg_ChargesExtraWeight()
    {
        // Act - GB: base 20m for 10kg, extra 2m per kg over
        // 12kg = 20 + (2 * 2) = 24m
        var result = _service.CalculateShippingCost("GB", "", 12m, "");

        // Assert
        Assert.Equal(24m, result);
    }

    [Fact]
    public void CalculateShippingCost_ForCountryWithoutRate_ReturnsZero()
    {
        // Act - Country not in database
        var result = _service.CalculateShippingCost("Japan", "", 5m, "");

        // Assert
        Assert.Equal(0m, result);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void CalculateShippingCost_WithEmptyCountry_ReturnsZero()
    {
        // Act
        var result = _service.CalculateShippingCost("", "NSW", 2m, "AustraliaPost");

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateShippingCost_WithNullCountry_ReturnsZero()
    {
        // Act
        var result = _service.CalculateShippingCost(null!, "NSW", 2m, "AustraliaPost");

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateShippingCost_WithZeroWeight_ReturnsCorrectCost()
    {
        // Act
        var result = _service.CalculateShippingCost("Australia", "NSW", 0m, "AustraliaPost");

        // Assert
        Assert.Equal(12.50m, result);
    }

    #endregion

    #region Available Methods Tests

    [Fact]
    public void GetAvailableShippingMethods_ForAustralia_ReturnsBothMethods()
    {
        // Act
        var result = _service.GetAvailableShippingMethods("Australia", "NSW");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("AustraliaPost", result);
        Assert.Contains("Courier", result);
    }

    [Fact]
    public void GetAvailableShippingMethods_ForUS_ReturnsInternationalCourier()
    {
        // Act
        var result = _service.GetAvailableShippingMethods("US", "");

        // Assert
        Assert.Single(result);
        Assert.Contains("International Courier", result);
    }

    [Fact]
    public void GetAvailableShippingMethods_ForCountryWithoutRate_ReturnsEmpty()
    {
        // Act
        var result = _service.GetAvailableShippingMethods("Japan", "");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableShippingMethods_ForEmptyCountry_ReturnsEmpty()
    {
        // Act
        var result = _service.GetAvailableShippingMethods("", "");

        // Assert
        Assert.Empty(result);
    }

    #endregion

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
