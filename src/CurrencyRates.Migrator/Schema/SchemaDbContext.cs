using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Migrator.Schema;

public sealed class SchemaDbContext(DbContextOptions<SchemaDbContext> options) : DbContext(options)
{
    public DbSet<CurrencyRow> Currencies => Set<CurrencyRow>();
    public DbSet<UserRow> Users => Set<UserRow>();
    public DbSet<FavoriteCurrencyRow> FavoriteCurrencies => Set<FavoriteCurrencyRow>();
    public DbSet<RevokedTokenRow> RevokedTokens => Set<RevokedTokenRow>();

    // описание всей схемы бд, остальные сервисы ее только используют
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyRow>(e =>
        {
            e.ToTable("currency");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).UseIdentityAlwaysColumn();
            e.Property(x => x.Code).HasMaxLength(3).IsRequired();
            e.Property(x => x.Name).HasMaxLength(128).IsRequired();
            e.Property(x => x.Rate).HasPrecision(18, 8);
            e.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<UserRow>(e =>
        {
            e.ToTable("user");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.Property(x => x.Password).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<FavoriteCurrencyRow>(e =>
        {
            e.ToTable("favorite_currency");
            e.HasKey(x => new { x.UserId, x.CurrencyId });
            e.HasOne<UserRow>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<CurrencyRow>().WithMany().HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RevokedTokenRow>(e =>
        {
            e.ToTable("revoked_token");
            e.HasKey(x => x.TokenId);
            e.Property(x => x.TokenId).HasMaxLength(64);
            e.HasIndex(x => x.ExpiresAt);
        });
    }
}
