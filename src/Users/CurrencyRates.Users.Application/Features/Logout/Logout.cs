using CurrencyRates.Users.Application.Abstractions;
using MediatR;

namespace CurrencyRates.Users.Application.Features.Logout;

public sealed record LogoutCommand(string TokenId, DateTimeOffset ExpiresAt) : IRequest;

public sealed class LogoutCommandHandler(IRevokedTokenRepository revokedTokens) : IRequestHandler<LogoutCommand>
{
    // логаут
    public Task Handle(LogoutCommand request, CancellationToken cancellationToken) =>
        revokedTokens.RevokeAsync(request.TokenId, request.ExpiresAt, cancellationToken);
}
