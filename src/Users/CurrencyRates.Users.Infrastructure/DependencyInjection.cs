using CurrencyRates.Common.Authentication;
using CurrencyRates.Users.Application.Abstractions;
using CurrencyRates.Users.Infrastructure.Persistence;
using CurrencyRates.Users.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CurrencyRates.Users.Infrastructure;

public static class DependencyInjection
{
    // регистрация бд, репозиториев и сервисов
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
                               ?? throw new InvalidOperationException("Не задана строка подключения Postgres.");

        services.AddDbContext<UsersDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<RevokedTokenRepository>();
        services.AddScoped<IRevokedTokenRepository>(sp => sp.GetRequiredService<RevokedTokenRepository>());
        services.AddScoped<ITokenRevocationChecker>(sp => sp.GetRequiredService<RevokedTokenRepository>());

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddHostedService<RevokedTokensCleanupService>();

        return services;
    }
}
