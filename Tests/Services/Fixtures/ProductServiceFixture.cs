using Microsoft.EntityFrameworkCore;
using NSubstitute;
using webapp.Data;
using webapp.Models;

namespace Tests.Services.Fixtures;

public class ProductServiceFixture
{
    public PetstoreContext Context { get; }
    public List<Pet> TestPets { get; }
    public List<Country> TestCountries { get; }
    public List<PetShippingRestriction> TestRestrictions { get; }

    public ProductServiceFixture()
    {
        var options = new DbContextOptionsBuilder<PetstoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new PetstoreContext(options);

        TestCountries = new List<Country>
        {
            new() { Id = 1, Code = "AU", Name = "Australia" },
            new() { Id = 2, Code = "US", Name = "United States" },
            new() { Id = 3, Code = "GB", Name = "United Kingdom" }
        };

        TestPets = new List<Pet>
        {
            new() 
            { 
                Id = 1, 
                Name = "Fluffy Cat", 
                Description = "A cute white cat",
                Species = "Cat",
                Price = 50m, 
                StockQuantity = 5,
                ImageUrl = "fluffy.jpg"
            },
            new() 
            { 
                Id = 2, 
                Name = "Buddy Dog", 
                Description = "A friendly golden retriever",
                Species = "Dog",
                Price = 300m, 
                StockQuantity = 2,
                ImageUrl = "buddy.jpg"
            },
            new() 
            { 
                Id = 3, 
                Name = "Tropical Fish", 
                Description = "Colorful aquatic pet",
                Species = "Fish",
                Price = 15m, 
                StockQuantity = 20,
                ImageUrl = "fish.jpg"
            }
        };

        TestRestrictions = new List<PetShippingRestriction>
        {
            new() { Id = 1, PetId = 2, CountryId = 3 } // Buddy Dog restricted to GB
        };

        Context.Countries.AddRange(TestCountries);
        Context.Pets.AddRange(TestPets);
        Context.PetShippingRestrictions.AddRange(TestRestrictions);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
