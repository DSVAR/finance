using CurrencyRates.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Users.Infrastructure.Persistence;

public sealed class RevokedToken
{
    public string TokenId { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("user")
            .Property(x => x.PasswordHash).HasColumnName("password");

        modelBuilder.Entity<RevokedToken>().ToTable("revoked_token")
            .HasKey(x => x.TokenId);
    }
}
