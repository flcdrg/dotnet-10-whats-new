using NSubstitute;
using Xunit;
using webapp.Services;

namespace Tests.Services;

public class GstCalculationServiceTests
{
    private readonly GstCalculationService _service = new();

    [Fact]
    public void CalculateGst_WithAustralianCountry_ReturnsCorrectTax()
    {
        // Arrange
        const decimal subtotal = 100m;
        const string country = "Australia";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(10m, result);
    }

    [Fact]
    public void CalculateGst_WithAustraliaLowercase_ReturnsCorrectTax()
    {
        // Arrange
        const decimal subtotal = 100m;
        const string country = "australia";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(10m, result);
    }

    [Fact]
    public void CalculateGst_WithAUSTRALIAUppercase_ReturnsCorrectTax()
    {
        // Arrange
        const decimal subtotal = 100m;
        const string country = "AUSTRALIA";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(10m, result);
    }

    [Fact]
    public void CalculateGst_WithNonAustralianCountry_ReturnsZero()
    {
        // Arrange
        const decimal subtotal = 100m;
        const string country = "United States";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateGst_WithNullCountry_ReturnsZero()
    {
        // Arrange
        const decimal subtotal = 100m;
        string? country = null;

        // Act
        var result = _service.CalculateGst(subtotal, country!);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateGst_WithEmptyCountry_ReturnsZero()
    {
        // Arrange
        const decimal subtotal = 100m;
        const string country = "";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateGst_WithZeroSubtotal_ReturnsZero()
    {
        // Arrange
        const decimal subtotal = 0m;
        const string country = "Australia";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateGst_WithNegativeSubtotal_ReturnsNegativeTax()
    {
        // Arrange
        const decimal subtotal = -100m;
        const string country = "Australia";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(-10m, result);
    }

    [Fact]
    public void CalculateGst_WithDecimalSubtotal_ReturnsCorrectTax()
    {
        // Arrange
        const decimal subtotal = 99.99m;
        const string country = "Australia";

        // Act
        var result = _service.CalculateGst(subtotal, country);

        // Assert
        Assert.Equal(9.999m, result);
    }
}
