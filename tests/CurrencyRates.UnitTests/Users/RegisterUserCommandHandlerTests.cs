using CurrencyRates.Common.Exceptions;
using CurrencyRates.Users.Application.Abstractions;
using CurrencyRates.Users.Application.Features.Register;
using CurrencyRates.Users.Domain;
using Microsoft.Extensions.Time.Testing;

namespace CurrencyRates.UnitTests.Users;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _hasher.Hash("secret-password").Returns("hashed");
        _handler = new RegisterUserCommandHandler(_users, _hasher, _time);
    }

    // успешная регистрация, пароль хешируется
    [Fact]
    public async Task Handle_NewUser_SavesUserWithHashedPassword()
    {
        var result = await _handler.Handle(new RegisterUserCommand("alice", "secret-password"), CancellationToken.None);

        Assert.Equal("alice", result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);
        await _users.Received(1).AddAsync(
            Arg.Is<User>(u => u.Id == result.Id && u.Name == "alice" 
                                                && u.PasswordHash == "hashed"
                                                && u.CreatedAt == _time.GetUtcNow()),
            Arg.Any<CancellationToken>());
    }

    // занятое имя - 409
    [Fact]
    public async Task Handle_NameTaken_ThrowsConflict()
    {
        _users.ExistsAsync("alice", Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(new RegisterUserCommand("alice", "secret-password"), CancellationToken.None));

        await _users.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    // некорректные имя или пароль
    [Theory]
    [InlineData("al", "secret-password")]
    [InlineData("alice smith", "secret-password")]
    [InlineData("алиса", "secret-password")]
    [InlineData("alice", "short")]
    [InlineData("", "")]
    public void Validator_InvalidInput_Fails(string name, string password)
    {
        var result = new RegisterUserCommandValidator().Validate(new RegisterUserCommand(name, password));

        Assert.False(result.IsValid);
    }

    // корректные данные проходят валидацию
    [Fact]
    public void Validator_ValidInput_Passes()
    {
        var result = new RegisterUserCommandValidator()
            .Validate(new RegisterUserCommand("alice_01", "secret-password"));

        Assert.True(result.IsValid);
    }
}
