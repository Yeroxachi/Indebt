namespace Domain.Entities;

public class Notification : Base
{
    public Guid DebtId { get; set; }
    public Debt Debt { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
    public bool IsRead { get; set; }
}