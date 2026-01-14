using Mediator;
using NSubstitute;
using webapp.Services;

namespace Tests.Application.Fixtures;

public class CommandHandlerFixture
{
    public IProductService ProductService { get; set; } = Substitute.For<IProductService>();
    public ICountryService CountryService { get; set; } = Substitute.For<ICountryService>();
    public IGstCalculationService GstService { get; set; } = Substitute.For<IGstCalculationService>();
    public IShippingCalculationService ShippingService { get; set; } = Substitute.For<IShippingCalculationService>();
    public ITimeProvider TimeProvider { get; set; } = Substitute.For<ITimeProvider>();
}
