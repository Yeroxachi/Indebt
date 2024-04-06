namespace Application.Responses;

public record DebtResponse
{
    public required Guid Id { get; init; }
    public required Guid LenderId { get; init; }
    public required Guid BorrowerId { get; init; }
    public required Guid GroupId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required DateTime CreatedDate { get; init; }
    public required decimal Remainder { get; init; }
    public required bool Approved { get; init; }
    public required bool Completed { get; init; }
    public DateTime? EndDate { get; init; }
    public DateTime? ReminderDate { get; init; }
}