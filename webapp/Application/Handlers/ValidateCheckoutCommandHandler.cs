using Mediator;
using webapp.Application.Commands;
using webapp.Services;

namespace webapp.Application.Handlers;

public class ValidateCheckoutCommandHandler : IRequestHandler<ValidateCheckoutCommand, ValidateCheckoutResult>
{
    private readonly ICountryService _countryService;
    private readonly IProductService _productService;
    private readonly IShippingCalculationService _shippingService;

    public ValidateCheckoutCommandHandler(
        ICountryService countryService,
        IProductService productService,
        IShippingCalculationService shippingService)
    {
        _countryService = countryService;
        _productService = productService;
        _shippingService = shippingService;
    }

    public async ValueTask<ValidateCheckoutResult> Handle(ValidateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var result = new ValidateCheckoutResult { IsValid = true };

        var selectedCountry = await _countryService.GetCountryByCodeAsync(request.Country);
        if (selectedCountry == null)
        {
            result.Errors["country"] = "Please select a valid country.";
            result.IsValid = false;
        }

        if (!request.Country.Equals("AU", StringComparison.OrdinalIgnoreCase))
        {
            request.State = string.Empty;
        }
        else if (string.IsNullOrWhiteSpace(request.State))
        {
            result.Errors["state"] = "State is required for Australian addresses.";
            result.IsValid = false;
        }

        var restrictedItems = new List<string>();
        if (request.Items != null)
        {
            foreach (var item in request.Items)
            {
                if (await _productService.IsPetRestrictedAsync(item.PetId, request.Country))
                {
                    restrictedItems.Add(item.PetName);
                }
            }

            if (restrictedItems.Count > 0)
            {
                var countryName = selectedCountry?.Name ?? request.Country;
                result.Errors["items"] = $"The following items cannot be shipped to {countryName}: {string.Join(", ", restrictedItems)}";
                result.IsValid = false;
            }
        }

        var availableMethods = _shippingService.GetAvailableShippingMethods(request.Country, request.State) ?? new List<string>();
        if (!availableMethods.Contains(request.ShippingMethod))
        {
            result.Errors["shippingMethod"] = "Please select a valid shipping method.";
            result.IsValid = false;
        }

        return result;
    }
}
