using CurrencyRates.Common.Exceptions;
using CurrencyRates.Finance.Application.Abstractions;
using CurrencyRates.Finance.Domain;
using FluentValidation;
using MediatR;

namespace CurrencyRates.Finance.Application.Features.Favorites;

public sealed record AddFavoriteCurrencyCommand(Guid UserId, string Code) : IRequest;

public sealed class AddFavoriteCurrencyCommandValidator : AbstractValidator<AddFavoriteCurrencyCommand>
{
    public AddFavoriteCurrencyCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches("^[A-Za-z]{3}$").WithMessage("Код валюты должен состоять из трех латинских букв, например USD.");
    }
}

public sealed class AddFavoriteCurrencyCommandHandler(ICurrencyRepository currencies, IFavoriteCurrencyRepository favorites)
    : IRequestHandler<AddFavoriteCurrencyCommand>
{
    // добавление валюты в избранное
    public async Task Handle(AddFavoriteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.ToUpperInvariant();
        var currency = await currencies.FindByCodeAsync(code, cancellationToken)
                       ?? throw new NotFoundException($"Валюта {code} не найдена.");

        await favorites.AddAsync(new FavoriteCurrency(request.UserId, currency.Id), cancellationToken);
    }
}
