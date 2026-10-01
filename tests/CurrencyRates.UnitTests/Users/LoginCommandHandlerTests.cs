using CurrencyRates.Common.Exceptions;
using CurrencyRates.Users.Application.Abstractions;
using CurrencyRates.Users.Application.Features.Login;
using CurrencyRates.Users.Domain;

namespace CurrencyRates.UnitTests.Users;

public sealed class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly LoginCommandHandler _handler;
    private readonly User _user = User.Create("alice", "hashed", DateTimeOffset.UtcNow);

    public LoginCommandHandlerTests()
    {
        _users.FindByNameAsync("alice", Arg.Any<CancellationToken>()).Returns(_user);
        _handler = new LoginCommandHandler(_users, _hasher, _tokens);
    }

    // успешный логин
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var token = new AccessTokenDto("jwt", DateTimeOffset.UtcNow.AddHours(1));
        _hasher.Verify("secret-password", "hashed").Returns(true);
        _tokens.CreateAccessToken(_user).Returns(token);

        var result = await _handler
            .Handle(new LoginCommand("alice", "secret-password"), CancellationToken.None);

        Assert.Same(token, result);
    }

    // неверный пароль
    [Fact]
    public async Task Handle_WrongPassword_ThrowsAuthenticationFailed()
    {
        _hasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            _handler.Handle(new LoginCommand("alice", "wrong-password"), CancellationToken.None));

        _tokens.DidNotReceiveWithAnyArgs().CreateAccessToken(default!);
    }

    // несуществующий пользователь
    [Fact]
    public async Task Handle_UnknownUser_ThrowsAuthenticationFailed()
    {
        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            _handler.Handle(new LoginCommand("bob", "secret-password"), CancellationToken.None));

        _hasher.DidNotReceiveWithAnyArgs().Verify(default!, default!);
    }
}
