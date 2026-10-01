using CurrencyRates.Finance.Application.Abstractions;
using CurrencyRates.Finance.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Finance.Infrastructure.Persistence;

public sealed class CurrencyRepository(FinanceDbContext db) : ICurrencyRepository
{
    // поиск валюты
    public Task<Currency?> FindByCodeAsync(string code, CancellationToken cancellationToken) =>
        db.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
}
