namespace CurrencyRates.Migrator.Schema;

public sealed class CurrencyRow
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class UserRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class FavoriteCurrencyRow
{
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }
}

public sealed class RevokedTokenRow
{
    public string TokenId { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}
