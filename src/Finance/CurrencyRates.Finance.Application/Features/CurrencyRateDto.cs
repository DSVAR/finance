using CurrencyRates.Finance.Domain;

namespace CurrencyRates.Finance.Application.Features;

public sealed record CurrencyRateDto(string Code, string Name, decimal Rate, DateTimeOffset UpdatedAt)
{
    // маппинг из доменной модели
    public static CurrencyRateDto From(Currency currency) =>
        new(currency.Code, currency.Name, currency.Rate, currency.UpdatedAt);
}
