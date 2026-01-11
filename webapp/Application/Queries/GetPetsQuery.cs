using Mediator;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Queries;

public class GetPetsQuery : IRequest<List<Pet>>
{
}

public class GetPetsQueryHandler : IRequestHandler<GetPetsQuery, List<Pet>>
{
    private readonly IProductService _productService;

    public GetPetsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async ValueTask<List<Pet>> Handle(GetPetsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetAllProductsAsync();
    }
}
