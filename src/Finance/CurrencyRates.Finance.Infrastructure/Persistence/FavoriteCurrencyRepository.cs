using CurrencyRates.Finance.Application.Abstractions;
using CurrencyRates.Finance.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Finance.Infrastructure.Persistence;

public sealed class FavoriteCurrencyRepository(FinanceDbContext db) : IFavoriteCurrencyRepository
{
    // избранные валюты пользователя
    public async Task<IReadOnlyList<Currency>> GetByUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await (from favorite in db.FavoriteCurrencies
               join currency in db.Currencies on favorite.CurrencyId equals currency.Id
               where favorite.UserId == userId
               orderby currency.Code
               select currency)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    // on conflict, чтобы двойной клик не превращался в 500
    public Task AddAsync(FavoriteCurrency favorite, CancellationToken cancellationToken) =>
        db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO favorite_currency (user_id, currency_id) VALUES ({favorite.UserId}, {favorite.CurrencyId}) ON CONFLICT DO NOTHING",
            cancellationToken);
}
