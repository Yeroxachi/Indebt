namespace Application.Responses;

public record UserResponse
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Name { get; init; } 
    public required string Surname { get; init; } 
    public required string Email { get; init; } 
    public required bool IsConfirmedEmail { get; init; }
}