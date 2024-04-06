namespace Application.DTOs;

public record EmailConfirmationDto
{
    public required string Email { get; init; }
    public required string Code { get; init; }
}