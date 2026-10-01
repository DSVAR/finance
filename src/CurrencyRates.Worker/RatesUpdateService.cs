using CurrencyRates.Worker.Cbr;
using CurrencyRates.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CurrencyRates.Worker;

public sealed class RatesUpdateService(
    IServiceScopeFactory scopeFactory,
    IOptions<RatesLoaderOptions> options,
    ILogger<RatesUpdateService> logger) : BackgroundService
{
    // первое обновление сразу на старте, дальше по таймеру
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.Interval);

        do
        {
            await RunOnceAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    // одно обновление курсов, ошибка не роняет сервис
    private async Task RunOnceAsync(CancellationToken stoppingToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var cbrClient = scope.ServiceProvider.GetRequiredService<CbrRatesClient>();
            var db = scope.ServiceProvider.GetRequiredService<RatesDbContext>();

            await UpdateAsync(cbrClient, db, stoppingToken);
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Не удалось обновить курсы, следующая попытка через {Interval}", options.Value.Interval);
        }
    }

    // скачивание курсов и upsert в таблицу currency
    private async Task UpdateAsync(CbrRatesClient cbrClient, RatesDbContext db, CancellationToken cancellationToken)
    {
        var rates = await cbrClient.GetDailyRatesAsync(cancellationToken);
        if (rates.Count == 0)
        {
            logger.LogWarning("ЦБ вернул пустой список валют, обновление пропущено");
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var existing = await db.Currencies.ToDictionaryAsync(c => c.Code, cancellationToken);
        var added = 0;

        foreach (var rate in rates)
        {
            if (!existing.TryGetValue(rate.Code, out var currency))
            {
                currency = new CurrencyEntity { Code = rate.Code };
                db.Currencies.Add(currency);
                existing[rate.Code] = currency;
                added++;
            }

            currency.Name = rate.Name;
            currency.Rate = rate.Rate;
            currency.UpdatedAt = now;
        }

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Курсы обновлены: {Total} валют, новых {Added}", rates.Count, added);
    }
}
