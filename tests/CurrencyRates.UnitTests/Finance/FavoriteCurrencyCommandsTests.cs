using CurrencyRates.Common.Exceptions;
using CurrencyRates.Finance.Application.Abstractions;
using CurrencyRates.Finance.Application.Features.Favorites;
using CurrencyRates.Finance.Domain;

namespace CurrencyRates.UnitTests.Finance;

public sealed class FavoriteCurrencyCommandsTests
{
    private readonly ICurrencyRepository _currencies = Substitute.For<ICurrencyRepository>();
    private readonly IFavoriteCurrencyRepository _favorites = Substitute.For<IFavoriteCurrencyRepository>();
    private readonly Guid _userId = Guid.NewGuid();

    public FavoriteCurrencyCommandsTests()
    {
        _currencies.FindByCodeAsync("USD", Arg.Any<CancellationToken>())
            .Returns(new Currency(7, "USD", "Доллар США", 92.5m, DateTimeOffset.UtcNow));
    }

    // добавление в избранное, регистр кода не важен
    [Fact]
    public async Task Add_KnownCurrency_AddsToFavoritesIgnoringCase()
    {
        var handler = new AddFavoriteCurrencyCommandHandler(_currencies, _favorites);

        await handler.Handle(new AddFavoriteCurrencyCommand(_userId, "usd"), CancellationToken.None);

        await _favorites.Received(1).AddAsync(
            Arg.Is<FavoriteCurrency>(f => f.UserId == _userId && f.CurrencyId == 7),
            Arg.Any<CancellationToken>());
    }

    // неизвестная валюта - 404
    [Fact]
    public async Task Add_UnknownCurrency_ThrowsNotFound()
    {
        var handler = new AddFavoriteCurrencyCommandHandler(_currencies, _favorites);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new AddFavoriteCurrencyCommand(_userId, "XXX"), CancellationToken.None));

        await _favorites.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    // неверный код валюты
    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("USDT")]
    [InlineData("U5D")]
    [InlineData("ДОЛ")]
    public void Validator_InvalidCode_Fails(string code)
    {
        var result = new AddFavoriteCurrencyCommandValidator().Validate(new AddFavoriteCurrencyCommand(_userId, code));

        Assert.False(result.IsValid);
    }

    // пустой пользователь не проходит валидацию
    [Fact]
    public void Validator_EmptyUser_Fails()
    {
        var result = new AddFavoriteCurrencyCommandValidator().
            Validate(new AddFavoriteCurrencyCommand(Guid.Empty, "USD"));

        Assert.False(result.IsValid);
    }
}
