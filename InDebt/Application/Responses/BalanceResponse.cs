using Domain.Enums;

namespace Application.Responses;

public record BalanceResponse
{
    public required BalanceType Type { get; init; }
    public required decimal Balance { get; init; }
    public required string CurrencyCode { get; init; }
    public required string GroupId { get; init; }
}