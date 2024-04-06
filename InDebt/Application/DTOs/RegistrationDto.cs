using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record RegistrationDto
{
    [MaxLength(100), MinLength(3)]
    public required string Username { get; init; }
    [MaxLength(100), MinLength(1)]
    public required string Name { get; init; }
    [MaxLength(100), MinLength(1)]
    public required string Surname { get; init; }
    [MaxLength(100), EmailAddress, MinLength(5)]
    public required string Email { get; init; }
    [MaxLength(30), MinLength(6)]
    public required string Password { get; init; }
}