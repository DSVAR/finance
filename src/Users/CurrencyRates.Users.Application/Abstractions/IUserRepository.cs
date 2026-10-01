using CurrencyRates.Users.Domain;

namespace CurrencyRates.Users.Application.Abstractions;

public interface IUserRepository
{
    // проверка занятости имени
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken);

    // поиск пользователя
    Task<User?> FindByNameAsync(string name, CancellationToken cancellationToken);

    // сохранение пользователя
    Task AddAsync(User user, CancellationToken cancellationToken);
}
