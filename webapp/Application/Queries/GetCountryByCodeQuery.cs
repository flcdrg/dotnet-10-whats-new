using Mediator;
using webapp.Models;
using webapp.Services;

namespace webapp.Application.Queries;

public class GetCountryByCodeQuery : IRequest<Country?>
{
    public string? Code { get; set; }

    public GetCountryByCodeQuery(string? code)
    {
        Code = code;
    }
}

public class GetCountryByCodeQueryHandler : IRequestHandler<GetCountryByCodeQuery, Country?>
{
    private readonly ICountryService _countryService;

    public GetCountryByCodeQueryHandler(ICountryService countryService)
    {
        _countryService = countryService;
    }

    public async ValueTask<Country?> Handle(GetCountryByCodeQuery request, CancellationToken cancellationToken)
    {
        return await _countryService.GetCountryByCodeAsync(request.Code);
    }
}
