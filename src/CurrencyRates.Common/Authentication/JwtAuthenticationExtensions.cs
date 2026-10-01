using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CurrencyRates.Common.Authentication;

public static class JwtAuthenticationExtensions
{
    // ITokenRevocationChecker каждый сервис регистрирует сам
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtOptions.SectionName);

        services.AddOptions<JwtOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwt = section.Get<JwtOptions>()
                  ?? throw new InvalidOperationException($"Не задана секция {JwtOptions.SectionName}.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwt.CreateSigningKey(),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Name
                };
                options.Events = new JwtBearerEvents { OnTokenValidated = RejectRevokedTokenAsync };
            });

        services.AddAuthorization();

        return services;
    }

    // отклонение токена, по которому уже был логаут
    private static async Task RejectRevokedTokenAsync(TokenValidatedContext context)
    {
        var tokenId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (string.IsNullOrEmpty(tokenId))
        {
            context.Fail("Токен без jti.");
            return;
        }

        var checker = context.HttpContext.RequestServices.GetRequiredService<ITokenRevocationChecker>();
        if (await checker.IsRevokedAsync(tokenId, context.HttpContext.RequestAborted))
            context.Fail("Токен отозван.");
    }
}
