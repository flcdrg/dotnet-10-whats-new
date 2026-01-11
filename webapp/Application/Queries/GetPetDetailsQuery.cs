using Mediator;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Queries;

public class GetPetDetailsQuery : IRequest<Pet?>
{
    public int PetId { get; set; }

    public GetPetDetailsQuery(int petId)
    {
        PetId = petId;
    }
}

public class GetPetDetailsQueryHandler : IRequestHandler<GetPetDetailsQuery, Pet?>
{
    private readonly IProductService _productService;

    public GetPetDetailsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async ValueTask<Pet?> Handle(GetPetDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductByIdAsync(request.PetId);
    }
}
