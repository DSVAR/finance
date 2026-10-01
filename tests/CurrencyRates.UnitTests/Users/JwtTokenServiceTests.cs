using CurrencyRates.Common.Authentication;
using CurrencyRates.Users.Domain;
using CurrencyRates.Users.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CurrencyRates.UnitTests.Users;

public sealed class JwtTokenServiceTests
{
    private readonly JwtOptions _options = new()
    {
        Issuer = "test-issuer",
        Audience = "test-audience",
        SigningKey = "unit-test-signing-key-0123456789abcdef",
        AccessTokenLifetime = TimeSpan.FromMinutes(30)
    };

    // токен валиден и содержит данные пользователя
    [Fact]
    public async Task CreateAccessToken_ProducesValidTokenWithUserClaims()
    {
        var now = DateTimeOffset.UtcNow;
        var service = new JwtTokenService(Options.Create(_options), new FakeTimeProvider(now));
        var user = User.Create("alice", "hashed", now);

        var result = service.CreateAccessToken(user);

        var validation = await new JsonWebTokenHandler()
            .ValidateTokenAsync(result.AccessToken, new TokenValidationParameters
        {
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience,
            IssuerSigningKey = _options.CreateSigningKey()
        });

        Assert.True(validation.IsValid);
        Assert.Equal(user.Id.ToString(), validation.Claims[JwtRegisteredClaimNames.Sub]);
        Assert.Equal("alice", validation.Claims[JwtRegisteredClaimNames.Name]);
        Assert.False(string.IsNullOrEmpty(validation.Claims[JwtRegisteredClaimNames.Jti] as string));
        Assert.Equal(now.AddMinutes(30).ToUnixTimeSeconds(), result.ExpiresAt.ToUnixTimeSeconds());
    }

    // у каждого токена свой jti
    [Fact]
    public void CreateAccessToken_EachCallHasOwnJti()
    {
        var service = new JwtTokenService(Options.Create(_options), TimeProvider.System);
        var user = User.Create("alice", "hashed", DateTimeOffset.UtcNow);
        var handler = new JsonWebTokenHandler();

        var first = handler.ReadJsonWebToken(service.CreateAccessToken(user).AccessToken).Id;
        var second = handler.ReadJsonWebToken(service.CreateAccessToken(user).AccessToken).Id;

        Assert.NotEqual(first, second);
    }
}
