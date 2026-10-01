using CurrencyRates.Common.Authentication;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Finance.Infrastructure.Persistence;

public sealed class TokenRevocationChecker(FinanceDbContext db) : ITokenRevocationChecker
{
    // проверка отзыва токена
    public Task<bool> IsRevokedAsync(string tokenId, CancellationToken cancellationToken) =>
        db.RevokedTokens.AnyAsync(t => t.TokenId == tokenId, cancellationToken);
}
