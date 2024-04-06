namespace Domain.Models;

public record ExchangeRateServiceOptions
{
    public string Host { get; init; }
    public string ApiKey { get; init; }
    public string PairConversionEndpoint { get; init; }
}