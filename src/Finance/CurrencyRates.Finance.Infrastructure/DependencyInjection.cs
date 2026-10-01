using CurrencyRates.Common.Authentication;
using CurrencyRates.Finance.Application.Abstractions;
using CurrencyRates.Finance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyRates.Finance.Infrastructure;

public static class DependencyInjection
{
    // регистрация бд и репозиториев
    public static IServiceCollection AddFinanceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
                               ?? throw new InvalidOperationException("Не задана строка подключения Postgres.");

        services.AddDbContext<FinanceDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IFavoriteCurrencyRepository, FavoriteCurrencyRepository>();
        services.AddScoped<ITokenRevocationChecker, TokenRevocationChecker>();

        return services;
    }
}
