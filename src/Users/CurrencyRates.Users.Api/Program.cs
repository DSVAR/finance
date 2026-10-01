using CurrencyRates.Common.Authentication;
using CurrencyRates.Common.Errors;
using CurrencyRates.Common.Swagger;
using CurrencyRates.Users.Application;
using CurrencyRates.Users.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddUsersApplication()
    .AddUsersInfrastructure(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddApiErrorHandling()
    .AddSwaggerWithJwt("Users API");

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
