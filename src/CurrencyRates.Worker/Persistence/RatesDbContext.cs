using Microsoft.EntityFrameworkCore;

namespace CurrencyRates.Worker.Persistence;

public sealed class CurrencyEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class RatesDbContext(DbContextOptions<RatesDbContext> options) : DbContext(options)
{
    public DbSet<CurrencyEntity> Currencies => Set<CurrencyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<CurrencyEntity>().ToTable("currency");
}
