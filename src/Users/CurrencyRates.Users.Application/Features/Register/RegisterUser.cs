using CurrencyRates.Common.Exceptions;
using CurrencyRates.Users.Application.Abstractions;
using CurrencyRates.Users.Domain;
using FluentValidation;
using MediatR;

namespace CurrencyRates.Users.Application.Features.Register;

public sealed record RegisterUserCommand(string Name, string Password) : IRequest<UserDto>;

public sealed record UserDto(Guid Id, string Name);

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 64)
            .Matches("^[A-Za-z0-9_.-]+$").WithMessage("Имя может содержать только латиницу, цифры и символы _ . -");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 128);
    }
}

public sealed class RegisterUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher, TimeProvider timeProvider)
    : IRequestHandler<RegisterUserCommand, UserDto>
{
    // регистрация
    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await users.ExistsAsync(request.Name, cancellationToken))
            throw new ConflictException($"Пользователь {request.Name} уже существует.");

        var user = User.Create(request.Name, passwordHasher.Hash(request.Password), timeProvider.GetUtcNow());
        await users.AddAsync(user, cancellationToken);

        return new UserDto(user.Id, user.Name);
    }
}
