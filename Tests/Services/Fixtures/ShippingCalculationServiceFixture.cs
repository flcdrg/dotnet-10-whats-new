using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace Tests.Services.Fixtures;

public class ShippingCalculationServiceFixture
{
    public PetstoreContext Context { get; }
    public List<Country> TestCountries { get; }
    public List<AustralianShippingRate> AustralianRates { get; }
    public List<InternationalShippingRate> InternationalRates { get; }

    public ShippingCalculationServiceFixture()
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

        AustralianRates = new List<AustralianShippingRate>
        {
            new() { Id = 1, State = "NSW", ShippingMethod = "AustraliaPost", Rate = 12.50m },
            new() { Id = 2, State = "NSW", ShippingMethod = "Courier", Rate = 22.50m },
            new() { Id = 3, State = "VIC", ShippingMethod = "AustraliaPost", Rate = 12.50m },
            new() { Id = 4, State = "VIC", ShippingMethod = "Courier", Rate = 22.50m },
            new() { Id = 5, State = "QLD", ShippingMethod = "AustraliaPost", Rate = 15.50m },
            new() { Id = 6, State = "QLD", ShippingMethod = "Courier", Rate = 25.50m }
        };

        InternationalRates = new List<InternationalShippingRate>
        {
            new() { Id = 1, CountryId = 2, Rate1To10Kg = 25m, RateOver10Kg = 37.5m },
            new() { Id = 2, CountryId = 3, Rate1To10Kg = 20m, RateOver10Kg = 24m }
        };

        Context.Countries.AddRange(TestCountries);
        Context.AustralianShippingRates.AddRange(AustralianRates);
        Context.InternationalShippingRates.AddRange(InternationalRates);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
