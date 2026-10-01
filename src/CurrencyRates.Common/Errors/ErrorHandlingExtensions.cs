using Microsoft.Extensions.DependencyInjection;

namespace CurrencyRates.Common.Errors;

public static class ErrorHandlingExtensions
{
    // обработка ошибок api
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();
        return services;
    }
}
