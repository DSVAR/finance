namespace CurrencyRates.Users.Application.Abstractions;

public interface IRevokedTokenRepository
{
    // отзыв токена
    Task RevokeAsync(string tokenId, DateTimeOffset expiresAt, CancellationToken cancellationToken);
}
