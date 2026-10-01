using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CurrencyRates.Migrator.Schema;

public sealed class DesignTimeSchemaDbContextFactory : IDesignTimeDbContextFactory<SchemaDbContext>
{
    // контекст для dotnet ef, к бд не подключается
    public SchemaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SchemaDbContext>()
            .UseNpgsql("Host=localhost;Database=currency_rates")
            .UseSnakeCaseNamingConvention()
            .Options;

        return new SchemaDbContext(options);
    }
}
