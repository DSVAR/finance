using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CurrencyRates.Common.Authentication;

public static class ClaimsPrincipalExtensions
{
    // id пользователя из токена
    public static Guid GetUserId(this ClaimsPrincipal principal) =>
        Guid.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id)
            ? id
            : throw new InvalidOperationException("В токене нет идентификатора пользователя.");

    // id токена
    public static string GetTokenId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(JwtRegisteredClaimNames.Jti)
        ?? throw new InvalidOperationException("В токене нет jti.");

    // срок действия токена
    public static DateTimeOffset GetTokenExpiration(this ClaimsPrincipal principal) =>
        DateTimeOffset.FromUnixTimeSeconds(long.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Exp)!));
}
