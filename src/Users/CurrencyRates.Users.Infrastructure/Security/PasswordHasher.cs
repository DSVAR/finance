using CurrencyRates.Users.Application.Abstractions;
using CurrencyRates.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace CurrencyRates.Users.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    // хеширование пароля
    public string Hash(string password) => _hasher.HashPassword(null!, password);

    // проверка пароля
    public bool Verify(string password, string passwordHash) =>
        _hasher.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}
