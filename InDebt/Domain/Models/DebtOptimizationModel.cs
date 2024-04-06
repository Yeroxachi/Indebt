namespace Domain.Models;

public record DebtOptimizationModel
{
    public Guid LenderId { get; init; }
    public Guid BorrowerId { get; init; }
    public Guid GroupId { get; init; }
    public decimal Amount { get; init; }
    public Guid CurrencyId { get; init; }
}