using CurrencyRates.Common.Behaviors;
using CurrencyRates.Users.Application.Features.Register;
using FluentValidation;

namespace CurrencyRates.UnitTests.Common;

public sealed class ValidationBehaviorTests
{
    // невалидные данные не доходят до хендлера
    [Fact]
    public async Task Handle_InvalidRequest_ThrowsAndSkipsHandler()
    {
        var behavior = new ValidationBehavior<RegisterUserCommand, UserDto>(new[] 
            { new RegisterUserCommandValidator() });
        var handlerCalled = false;

        await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(
            new RegisterUserCommand("", ""),
            () =>
            {
                handlerCalled = true;
                return Task.FromResult(new UserDto(Guid.NewGuid(), "x"));
            },
            CancellationToken.None));

        Assert.False(handlerCalled);
    }

    // валидные данные доходят до хендлера
    [Fact]
    public async Task Handle_ValidRequest_CallsHandler()
    {
        var behavior = new ValidationBehavior<RegisterUserCommand, UserDto>(new[] { new RegisterUserCommandValidator() });
        var expected = new UserDto(Guid.NewGuid(), "alice");

        var result = await behavior.Handle(new RegisterUserCommand("alice", "secret-password"), 
            () => Task.FromResult(expected), CancellationToken.None);

        Assert.Same(expected, result);
    }
}
