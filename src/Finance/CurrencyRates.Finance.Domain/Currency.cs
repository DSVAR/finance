namespace CurrencyRates.Finance.Domain;

public sealed class Currency(int id, string code, string name, decimal rate, DateTimeOffset updatedAt)
{
    public int Id { get; private set; } = id;

    public string Code { get; private set; } = code;

    public string Name { get; private set; } = name;

    public decimal Rate { get; private set; } = rate;

    public DateTimeOffset UpdatedAt { get; private set; } = updatedAt;
}
