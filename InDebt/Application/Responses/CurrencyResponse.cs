namespace Application.Responses;

public record CurrencyResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string CurrencyCode { get; init; }
}