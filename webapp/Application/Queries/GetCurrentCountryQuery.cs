using Mediator;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Queries;

public class GetCurrentCountryQuery : IRequest<string>
{
}

public class GetCurrentCountryQueryHandler : IRequestHandler<GetCurrentCountryQuery, string>
{
    private readonly ICountryService _countryService;

    public GetCurrentCountryQueryHandler(ICountryService countryService)
    {
        _countryService = countryService;
    }

    public async ValueTask<string> Handle(GetCurrentCountryQuery request, CancellationToken cancellationToken)
    {
        return await _countryService.GetCurrentCountryCodeAsync();
    }
}
