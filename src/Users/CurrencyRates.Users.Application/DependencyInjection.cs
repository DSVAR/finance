using System.Globalization;
using CurrencyRates.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyRates.Users.Application;

public static class DependencyInjection
{
    // регистрация слоя приложения
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(assembly);
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.GetCultureInfo("ru");

        return services;
    }
}
