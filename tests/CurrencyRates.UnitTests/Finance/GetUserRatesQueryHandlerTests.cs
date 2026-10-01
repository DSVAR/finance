using CurrencyRates.Finance.Application.Abstractions;
using CurrencyRates.Finance.Application.Features.GetUserRates;
using CurrencyRates.Finance.Domain;

namespace CurrencyRates.UnitTests.Finance;

public sealed class GetUserRatesQueryHandlerTests
{
    private static readonly DateTimeOffset UpdatedAt 
        = new(2026, 1, 15, 9, 0, 0, TimeSpan.Zero);

    // курсы избранных валют пользователя
    [Fact]
    public async Task Handle_ReturnsRatesOfUserFavorites()
    {
        var userId = Guid.NewGuid();
        var favorites = Substitute.For<IFavoriteCurrencyRepository>();
        favorites.GetByUserAsync(userId, Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Currency(1, "EUR", "Евро", 100.12345678m, UpdatedAt),
            new Currency(2, "USD", "Доллар США", 92.5m, UpdatedAt)
        });

        var result = await new GetUserRatesQueryHandler(favorites)
            .Handle(new GetUserRatesQuery(userId), CancellationToken.None);

        Assert.Collection(result,
            eur =>
            {
                Assert.Equal("EUR", eur.Code);
                Assert.Equal("Евро", eur.Name);
                Assert.Equal(100.12345678m, eur.Rate);
                Assert.Equal(UpdatedAt, eur.UpdatedAt);
            },
            usd => Assert.Equal("USD", usd.Code));
    }

    // без избранного - пустой список
    [Fact]
    public async Task Handle_NoFavorites_ReturnsEmptyList()
    {
        var favorites = Substitute.For<IFavoriteCurrencyRepository>();
        favorites.GetByUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).
            Returns(Array.Empty<Currency>());

        var result = await new GetUserRatesQueryHandler(favorites)
            .Handle(new GetUserRatesQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Empty(result);
    }
}
