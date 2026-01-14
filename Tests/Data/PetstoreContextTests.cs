using Microsoft.EntityFrameworkCore;
using Xunit;
using webapp.Data;
using webapp.Models;

namespace Tests.Data;

public class PetstoreContextTests : IDisposable
{
    private readonly DbContextOptions<PetstoreContext> _dbContextOptions;
    private readonly PetstoreContext _context;

    public PetstoreContextTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<PetstoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PetstoreContext(_dbContextOptions);
    }

    [Fact]
    public void DbContext_CanBeCreated()
    {
        // Act & Assert
        Assert.NotNull(_context);
    }

    [Fact]
    public void OnModelCreating_LoadsSeedData_Countries()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Act
        var countries = context.Countries.ToList();

        // Assert
        Assert.NotEmpty(countries);
        Assert.Contains(countries, c => c.Code == "AU");
        Assert.Contains(countries, c => c.Name == "Australia");
    }

    [Fact]
    public void OnModelCreating_LoadsSeedData_Pets()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Act
        var pets = context.Pets.ToList();

        // Assert
        Assert.NotEmpty(pets);
    }

    [Fact]
    public void OnModelCreating_LoadsSeedData_AustralianShippingRates()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Act
        var rates = context.AustralianShippingRates.ToList();

        // Assert
        Assert.NotEmpty(rates);
        Assert.Contains(rates, r => r.State == "NSW");
    }

    [Fact]
    public void OnModelCreating_LoadsSeedData_InternationalShippingRates()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Act
        var rates = context.InternationalShippingRates.ToList();

        // Assert
        Assert.NotEmpty(rates);
    }

    [Fact]
    public void DbContext_CanInsertCountry()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();
        
        var newCountry = new Country { Code = "NZ", Name = "New Zealand" };

        // Act
        context.Countries.Add(newCountry);
        context.SaveChanges();

        // Assert
        var retrieved = context.Countries.FirstOrDefault(c => c.Code == "NZ");
        Assert.NotNull(retrieved);
        Assert.Equal("New Zealand", retrieved.Name);
    }

    [Fact]
    public void DbContext_CanInsertPet()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();
        
        var newPet = new Pet 
        { 
            Name = "New Pet", 
            Description = "A test pet",
            Species = "Test",
            Price = 99.99m,
            StockQuantity = 5,
            ImageUrl = "test.jpg"
        };

        // Act
        context.Pets.Add(newPet);
        context.SaveChanges();

        // Assert
        var retrieved = context.Pets.FirstOrDefault(p => p.Name == "New Pet");
        Assert.NotNull(retrieved);
        Assert.Equal("Test", retrieved.Species);
    }

    [Fact]
    public void DbContext_CanQueryPets_WithCountryRestrictions()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Act
        var pets = context.Pets.ToList();
        var restrictions = context.PetShippingRestrictions.ToList();

        // Assert
        Assert.NotEmpty(pets);
        // Restrictions may be populated from seed data
    }

    [Fact]
    public void DbContext_CountriesAreSortableByName()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Act
        var sortedCountries = context.Countries.OrderBy(c => c.Name).ToList();

        // Assert
        Assert.NotEmpty(sortedCountries);
        for (int i = 1; i < sortedCountries.Count; i++)
        {
            Assert.True(string.Compare(sortedCountries[i - 1].Name, sortedCountries[i].Name, StringComparison.OrdinalIgnoreCase) <= 0);
        }
    }

    [Fact]
    public void DbContext_PetsAreQueryableBySpecies()
    {
        // Arrange
        var context = new PetstoreContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Act
        var pets = context.Pets.ToList();
        var species = pets.Select(p => p.Species).Distinct().ToList();

        // Assert
        Assert.NotEmpty(species);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
