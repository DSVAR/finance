namespace CurrencyRates.Users.Domain;

public sealed class User
{
    private User(Guid id, string name, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    // создание пользователя
    public static User Create(string name, string passwordHash, DateTimeOffset createdAt) =>
        new(Guid.NewGuid(), name, passwordHash, createdAt);
}
