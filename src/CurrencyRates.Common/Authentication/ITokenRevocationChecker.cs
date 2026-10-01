namespace CurrencyRates.Common.Authentication;

public interface ITokenRevocationChecker
{
    // проверка отзыва токена
    Task<bool> IsRevokedAsync(string tokenId, CancellationToken cancellationToken);
}
