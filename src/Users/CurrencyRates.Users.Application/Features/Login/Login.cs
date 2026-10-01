using CurrencyRates.Common.Exceptions;
using CurrencyRates.Users.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace CurrencyRates.Users.Application.Features.Login;

public sealed record LoginCommand(string Name, string Password) : IRequest<AccessTokenDto>;

public sealed record AccessTokenDto(string AccessToken, DateTimeOffset ExpiresAt);

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginCommandHandler(IUserRepository users, IPasswordHasher passwordHasher, ITokenService tokenService)
    : IRequestHandler<LoginCommand, AccessTokenDto>
{
    // логин
    public async Task<AccessTokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await users.FindByNameAsync(request.Name, cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new AuthenticationFailedException("Неверное имя пользователя или пароль.");

        return tokenService.CreateAccessToken(user);
    }
}
