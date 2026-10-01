using System.Text;
using CurrencyRates.Worker;
using CurrencyRates.Worker.Cbr;
using CurrencyRates.Worker.Persistence;
using Microsoft.EntityFrameworkCore;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

var connectionString = builder.Configuration.GetConnectionString("Postgres")
                       ?? throw new InvalidOperationException("Не задана строка подключения Postgres.");

builder.Services.Configure<RatesLoaderOptions>(builder.Configuration.GetSection("RatesLoader"));

builder.Services.AddDbContext<RatesDbContext>(options => options
    .UseNpgsql(connectionString)
    .UseSnakeCaseNamingConvention());

builder.Services
    .AddHttpClient<CbrRatesClient>(client =>
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CurrencyRates/1.0");
    })
    .AddStandardResilienceHandler();

builder.Services.AddHostedService<RatesUpdateService>();

builder.Build().Run();
