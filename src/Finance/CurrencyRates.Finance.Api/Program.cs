using CurrencyRates.Common.Authentication;
using CurrencyRates.Common.Errors;
using CurrencyRates.Common.Swagger;
using CurrencyRates.Finance.Application;
using CurrencyRates.Finance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFinanceApplication()
    .AddFinanceInfrastructure(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddApiErrorHandling()
    .AddSwaggerWithJwt("Finance API");

builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
