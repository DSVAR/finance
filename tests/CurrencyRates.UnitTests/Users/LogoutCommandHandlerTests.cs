using CurrencyRates.Users.Application.Abstractions;
using CurrencyRates.Users.Application.Features.Logout;

namespace CurrencyRates.UnitTests.Users;

public sealed class LogoutCommandHandlerTests
{
    // логаут отзывает токен до конца его срока
    [Fact]
    public async Task Handle_RevokesTokenUntilItsExpiration()
    {
        var revokedTokens = Substitute.For<IRevokedTokenRepository>();
        var expiresAt = new DateTimeOffset(2026, 1, 15, 11, 0, 0, TimeSpan.Zero);
        var handler = new LogoutCommandHandler(revokedTokens);

        await handler.Handle(new LogoutCommand("token-id", expiresAt), CancellationToken.None);

        await revokedTokens.Received(1).RevokeAsync("token-id", expiresAt, Arg.Any<CancellationToken>());
    }
}
