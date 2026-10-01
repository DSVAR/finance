using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CurrencyRates.Users.Infrastructure.Persistence;

public sealed class RevokedTokensCleanupService(IServiceScopeFactory scopeFactory, ILogger<RevokedTokensCleanupService> logger)
    : BackgroundService
{
    // раз в час удаляет отозванные токены, у которых истек срок
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                var now = DateTimeOffset.UtcNow;

                var deleted = await db.RevokedTokens
                    .Where(t => t.ExpiresAt < now)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deleted > 0)
                    logger.LogInformation("Удалено истекших отозванных токенов: {Count}", deleted);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Не удалось очистить отозванные токены");
            }
        }
    }
}
