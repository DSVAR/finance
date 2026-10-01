using CurrencyRates.Finance.Domain;

namespace CurrencyRates.Finance.Application.Abstractions;

public interface IFavoriteCurrencyRepository
{
    // избранные валюты пользователя
    Task<IReadOnlyList<Currency>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);

    // добавление в избранное
    Task AddAsync(FavoriteCurrency favorite, CancellationToken cancellationToken);
}
