using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record LoginDto
{
    [MaxLength(100),MinLength(3)]
    public required string Username { get; init; }
    [MinLength(6), MaxLength(30)]
    public required string Password { get; init; }
}