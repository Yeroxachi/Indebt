using Domain.Enums;

namespace Application.Responses;

public record TransactionResponse
{
    public required Guid Id { get; init; }
    public required Guid LenderId { get; init; }
    public required Guid BorrowerId { get; init; }
    public required Guid DebtId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required bool Approved { get; init; }
    public string Type { get; private set; }

    public void SetTransactionType(Guid userId)
    {
        Type = (userId == LenderId ? TransactionType.Outgoing : TransactionType.Incoming).ToString();
    }
}