using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record ExchangeRateDto
{
    public required Guid LeftCurrencyId  { get; init; }
    public required Guid RightCurrencyId { get; init; }
    [Range(0, (double)decimal.MaxValue)]
    public required decimal Amount { get; set; }
}