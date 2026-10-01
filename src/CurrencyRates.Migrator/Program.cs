using CurrencyRates.Migrator.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

var connectionString = builder.Configuration.GetConnectionString("Postgres")
                       ?? throw new InvalidOperationException("Не задана строка подключения Postgres.");

builder.Services.AddDbContext<SchemaDbContext>(options => options
    .UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure(5))
    .UseSnakeCaseNamingConvention());

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    await using var scope = host.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<SchemaDbContext>();

    var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
    if (pending.Count == 0)
    {
        logger.LogInformation("Схема актуальна, миграций нет");
        return 0;
    }

    logger.LogInformation("Применяю миграции: {Migrations}", string.Join(", ", pending));
    await db.Database.MigrateAsync();
    logger.LogInformation("Миграции применены");

    return 0;
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Не удалось применить миграции");
    return 1;
}
