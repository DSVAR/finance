using CurrencyRates.Common.Authentication;
using CurrencyRates.Users.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Users.Infrastructure.Persistence;

public sealed class RevokedTokenRepository(UsersDbContext db) : IRevokedTokenRepository, ITokenRevocationChecker
{
    // повторный логаут тем же токеном не ошибка
    public Task RevokeAsync(string tokenId, DateTimeOffset expiresAt, CancellationToken cancellationToken) =>
        db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO revoked_token (token_id, expires_at) VALUES ({tokenId}, {expiresAt}) ON CONFLICT (token_id) DO NOTHING",
            cancellationToken);

    // проверка отзыва токена
    public Task<bool> IsRevokedAsync(string tokenId, CancellationToken cancellationToken) =>
        db.RevokedTokens.AnyAsync(t => t.TokenId == tokenId, cancellationToken);
}
