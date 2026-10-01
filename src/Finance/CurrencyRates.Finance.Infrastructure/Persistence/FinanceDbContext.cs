using CurrencyRates.Finance.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Finance.Infrastructure.Persistence;

public sealed class RevokedToken
{
    public string TokenId { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}

public sealed class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<FavoriteCurrency> FavoriteCurrencies => Set<FavoriteCurrency>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>().ToTable("currency");

        modelBuilder.Entity<FavoriteCurrency>().ToTable("favorite_currency")
            .HasKey(x => new { x.UserId, x.CurrencyId });

        modelBuilder.Entity<RevokedToken>().ToTable("revoked_token")
            .HasKey(x => x.TokenId);
    }
}
