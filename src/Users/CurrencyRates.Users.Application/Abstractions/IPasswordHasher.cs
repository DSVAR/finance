namespace CurrencyRates.Users.Application.Abstractions;

public interface IPasswordHasher
{
    // хеширование пароля
    string Hash(string password);

    // проверка пароля
    bool Verify(string password, string passwordHash);
}
