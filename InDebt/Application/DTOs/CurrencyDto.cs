using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record CurrencyDto
{
    [MaxLength(3)]
    public required string CurrencyCode { get; init; }
}