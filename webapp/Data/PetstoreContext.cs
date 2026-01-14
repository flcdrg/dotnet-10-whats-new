using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Data;

public class PetstoreContext : DbContext
{
    public PetstoreContext(DbContextOptions<PetstoreContext> options) : base(options)
    {
    }

    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<AustralianShippingRate> AustralianShippingRates { get; set; } = null!;
    public DbSet<InternationalShippingRate> InternationalShippingRates { get; set; } = null!;
    public DbSet<PetShippingRestriction> PetShippingRestrictions { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Code = "Australia", Name = "Australia" },
            new Country { Id = 2, Code = "UK", Name = "United Kingdom" },
            new Country { Id = 3, Code = "NewZealand", Name = "New Zealand" },
            new Country { Id = 4, Code = "Antarctica", Name = "Antarctica" }
        );

        // Seed Pet data
        modelBuilder.Entity<Pet>().HasData(
            new Pet { Id = 1, Name = "Fluffy", Species = "Cat", Description = "A soft and cuddly tabby cat", Price = 99.99m, ImageUrl = "https://images.unsplash.com/photo-1574158622682-e40e69881006?w=300&h=300&fit=crop", StockQuantity = 5, CreatedAt = DateTime.UtcNow },
            new Pet { Id = 2, Name = "Max", Species = "Dog", Description = "An energetic golden retriever", Price = 199.99m, ImageUrl = "https://images.unsplash.com/photo-1633722715463-d30f4f325e24?w=300&h=300&fit=crop", StockQuantity = 3, CreatedAt = DateTime.UtcNow },
            new Pet { Id = 3, Name = "Tweety", Species = "Bird", Description = "A colorful parakeet", Price = 49.99m, ImageUrl = "https://images.unsplash.com/photo-1552728089-57bdde30beb3?w=300&h=300&fit=crop", StockQuantity = 10, CreatedAt = DateTime.UtcNow },
            new Pet { Id = 4, Name = "Bubbles", Species = "Fish", Description = "A beautiful goldfish", Price = 29.99m, ImageUrl = "https://images.unsplash.com/photo-1520366498724-709889c0c685?w=300&h=300&fit=crop&auto=format&q=80", StockQuantity = 15, CreatedAt = DateTime.UtcNow }
        );

        // Seed Australian Shipping Rates
        var australianStates = new[] { "NSW", "VIC", "QLD", "WA", "SA", "TAS", "NT", "ACT" };
        var ausPostRates = australianStates.Select((state, index) => new AustralianShippingRate
        {
            Id = index + 1,
            State = state,
            ShippingMethod = "AustraliaPost",
            Rate = 10m
        }).ToList();

        var courierRates = new List<AustralianShippingRate>
        {
            new AustralianShippingRate { Id = 9, State = "NSW", ShippingMethod = "Courier", Rate = 18m },
            new AustralianShippingRate { Id = 10, State = "VIC", ShippingMethod = "Courier", Rate = 18m },
            new AustralianShippingRate { Id = 11, State = "QLD", ShippingMethod = "Courier", Rate = 22m },
            new AustralianShippingRate { Id = 12, State = "WA", ShippingMethod = "Courier", Rate = 28m },
            new AustralianShippingRate { Id = 13, State = "SA", ShippingMethod = "Courier", Rate = 20m },
            new AustralianShippingRate { Id = 14, State = "TAS", ShippingMethod = "Courier", Rate = 25m },
            new AustralianShippingRate { Id = 15, State = "NT", ShippingMethod = "Courier", Rate = 30m },
            new AustralianShippingRate { Id = 16, State = "ACT", ShippingMethod = "Courier", Rate = 15m }
        };

        ausPostRates.AddRange(courierRates);
        modelBuilder.Entity<AustralianShippingRate>().HasData(ausPostRates);

        modelBuilder.Entity<PetShippingRestriction>().HasData(
            new PetShippingRestriction { Id = 1, PetId = 4, CountryId = 3 }
        );

        // Seed International Shipping Rates
        modelBuilder.Entity<InternationalShippingRate>().HasData(
            new InternationalShippingRate { Id = 1, CountryId = 2, Rate1To10Kg = 35m, RateOver10Kg = 60m },
            new InternationalShippingRate { Id = 2, CountryId = 3, Rate1To10Kg = 25m, RateOver10Kg = 45m },
            new InternationalShippingRate { Id = 3, CountryId = 4, Rate1To10Kg = 50m, RateOver10Kg = 95m }
        );
    }
}
