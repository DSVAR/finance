namespace CurrencyRates.Finance.Domain;

public sealed class FavoriteCurrency(Guid userId, int currencyId)
{
    public Guid UserId { get; private set; } = userId;

    public int CurrencyId { get; private set; } = currencyId;
}
