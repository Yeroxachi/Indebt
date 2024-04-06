using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record DebtDto
{
    public required Guid BorrowerId { get; init; }
    public required Guid GroupId { get; init; }
    [Range(0, (double)decimal.MaxValue)]
    public required decimal Amount { get; init; }
    [StringLength(3)]
    public required string CurrencyCode { get; init; }
    public DateTime? EndDate { get; init; }
    public DateTime? ReminderDate { get; init; }
}