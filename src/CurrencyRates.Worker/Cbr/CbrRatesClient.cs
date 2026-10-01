using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace CurrencyRates.Worker.Cbr;

public sealed record CbrRate(string Code, string Name, decimal Rate);

public sealed class CbrRatesClient(HttpClient httpClient, IOptions<RatesLoaderOptions> options)
{
    // получение курсов с сайта цб
    public async Task<IReadOnlyList<CbrRate>> GetDailyRatesAsync(CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(options.Value.SourceUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

        return Parse(document);
    }

    // разбор xml цб, курс приводится к одной единице валюты
    private static List<CbrRate> Parse(XDocument document)
    {
        var rates = new List<CbrRate>();

        foreach (var valute in document.Descendants("Valute"))
        {
            var code = (string?)valute.Element("CharCode");
            var name = (string?)valute.Element("Name");
            var nominal = (string?)valute.Element("Nominal");
            var value = (string?)valute.Element("Value");

            if (code is null || name is null || nominal is null || value is null)
                throw new FormatException("Неожиданный формат ответа ЦБ.");

            var rate = ParseDecimal(value) / int.Parse(nominal);
            rates.Add(new CbrRate(code, name, Math.Round(rate, 8)));
        }

        return rates;
    }

    // цб отдает числа с запятой
    private static decimal ParseDecimal(string value) =>
        decimal.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
}
