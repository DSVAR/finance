using CurrencyRates.Finance.Domain;

namespace CurrencyRates.Finance.Application.Abstractions;

public interface ICurrencyRepository
{
    // поиск валюты
    Task<Currency?> FindByCodeAsync(string code, CancellationToken cancellationToken);
}
