using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record TransactionDto
{
    public required Guid DebtId { get; init; }
    [Range(0, (double)decimal.MaxValue)]
    public required decimal Amount { get; set; }
    [StringLength(3)]
    public required string CurrencyCode { get; init; }
}