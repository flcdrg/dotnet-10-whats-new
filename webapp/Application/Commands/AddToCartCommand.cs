using Mediator;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Commands;

public class AddToCartCommand : IRequest<AddToCartResult>
{
    public int PetId { get; set; }
    public int Quantity { get; set; }
    public string CountryCode { get; set; } = string.Empty;
}

public class AddToCartResult
{
    public bool Success { get; set; }
    public CartItem? Item { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, AddToCartResult>
{
    private readonly IProductService _productService;
    private readonly ICountryService _countryService;

    public AddToCartCommandHandler(IProductService productService, ICountryService countryService)
    {
        _productService = productService;
        _countryService = countryService;
    }

    public async ValueTask<AddToCartResult> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var pet = await _productService.GetProductByIdAsync(request.PetId, request.CountryCode);
        if (pet == null)
        {
            return new AddToCartResult
            {
                Success = false,
                ErrorMessage = "This pet is not available for your selected country."
            };
        }

        if (await _productService.IsPetRestrictedAsync(request.PetId, request.CountryCode))
        {
            var country = await _countryService.GetCountryByCodeAsync(request.CountryCode);
            var countryName = country?.Name ?? request.CountryCode;
            return new AddToCartResult
            {
                Success = false,
                ErrorMessage = $"{pet.Name} cannot be shipped to {countryName}."
            };
        }

        var cartItem = new CartItem
        {
            PetId = request.PetId,
            PetName = pet.Name,
            PetPrice = pet.Price,
            Quantity = request.Quantity,
            ImageUrl = pet.ImageUrl
        };

        return new AddToCartResult
        {
            Success = true,
            Item = cartItem
        };
    }
}
