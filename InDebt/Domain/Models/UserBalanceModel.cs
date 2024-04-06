namespace Domain.Models;

public record UserBalanceModel
{
    public Guid UserId { get; init; }
    public decimal Balance { get; set; }
    public Guid PriorityCurrencyId { get; init; } 
}