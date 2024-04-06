using Domain.Enums;

namespace Application.Responses;

public record GroupInviteResponse
{
    public required Guid Id { get; init; }
    public required Guid GroupId { get; set; }
    public required Guid InviterId { get; init; }
    public required Guid InvitedId { get; init; }
    public required InviteStatus InviteStatus { get; init; }
}