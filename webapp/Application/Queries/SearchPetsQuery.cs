using Mediator;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Queries;

public class SearchPetsQuery : IRequest<List<Pet>>
{
    public string SearchTerm { get; set; } = string.Empty;

    public SearchPetsQuery(string searchTerm)
    {
        SearchTerm = searchTerm;
    }
}

public class SearchPetsQueryHandler : IRequestHandler<SearchPetsQuery, List<Pet>>
{
    private readonly IProductService _productService;

    public SearchPetsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async ValueTask<List<Pet>> Handle(SearchPetsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.SearchProductsAsync(request.SearchTerm);
    }
}
