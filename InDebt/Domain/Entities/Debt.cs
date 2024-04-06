namespace Domain.Entities;

public class Debt : Base
{
    public Debt()
    {
        Transactions = new HashSet<Transaction>();
    }

    public Guid LenderId { get; set; }
    public User Lender { get; set; }
    public Guid BorrowerId { get; set; }
    public User Borrower { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public decimal Amount { get; set; }
    public decimal Remainder { get; set; }
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ReminderDate { get; set; }
    public bool Approved { get; set; }
    public bool Completed { get; set; }

    public ICollection<Transaction> Transactions { get; set; }
}