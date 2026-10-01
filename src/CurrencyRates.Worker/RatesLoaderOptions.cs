namespace CurrencyRates.Worker;

public sealed class RatesLoaderOptions
{
    public string SourceUrl { get; set; } = "http://www.cbr.ru/scripts/XML_daily.asp";

    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
}
