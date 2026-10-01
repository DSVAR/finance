using CurrencyRates.Finance.Application.Abstractions;
using MediatR;

namespace CurrencyRates.Finance.Application.Features.GetUserRates;

public sealed record GetUserRatesQuery(Guid UserId) : IRequest<IReadOnlyList<CurrencyRateDto>>;

public sealed class GetUserRatesQueryHandler(IFavoriteCurrencyRepository favorites)
    : IRequestHandler<GetUserRatesQuery, IReadOnlyList<CurrencyRateDto>>
{
    // курсы избранных валют
    public async Task<IReadOnlyList<CurrencyRateDto>> Handle(GetUserRatesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await favorites.GetByUserAsync(request.UserId, cancellationToken);
        return currencies.Select(CurrencyRateDto.From).ToList();
    }
}
