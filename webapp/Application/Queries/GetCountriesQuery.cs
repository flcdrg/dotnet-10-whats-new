using Mediator;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Queries;

public class GetCountriesQuery : IRequest<IReadOnlyList<Country>>
{
}

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, IReadOnlyList<Country>>
{
    private readonly ICountryService _countryService;

    public GetCountriesQueryHandler(ICountryService countryService)
    {
        _countryService = countryService;
    }

    public async ValueTask<IReadOnlyList<Country>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        return await _countryService.GetAvailableCountriesAsync();
    }
}
