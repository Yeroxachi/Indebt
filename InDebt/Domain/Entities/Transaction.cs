namespace Domain.Entities;

public class Transaction : Base
{
    public Guid DebtId { get; set; }
    public Debt Debt { get; set; }
    public decimal Amount { get; set; }
    public bool Approved { get; set; }
}