using CurrencyRates.Common.Exceptions;
using CurrencyRates.Users.Application.Abstractions;
using CurrencyRates.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CurrencyRates.Users.Infrastructure.Persistence;

public sealed class UserRepository(UsersDbContext db) : IUserRepository
{
    // проверка занятости имени
    public Task<bool> ExistsAsync(string name, CancellationToken cancellationToken) =>
        db.Users.AnyAsync(u => u.Name == name, cancellationToken);

    // поиск пользователя
    public Task<User?> FindByNameAsync(string name, CancellationToken cancellationToken) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == name, cancellationToken);

    // гонку двух регистраций с одним именем ловит уникальный индекс
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException($"Пользователь {user.Name} уже существует.");
        }
    }
}
