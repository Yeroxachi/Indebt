namespace Application.DTOs;

public record GroupInviteDto
{
    public required Guid GroupId { get; init; }
    public required Guid InvitedId { get; init; }
}