using CurrencyRates.Users.Application.Features.Login;
using CurrencyRates.Users.Domain;

namespace CurrencyRates.Users.Application.Abstractions;

public interface ITokenService
{
    // выпуск токена
    AccessTokenDto CreateAccessToken(User user);
}
