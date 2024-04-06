namespace Application.Responses;

public record MergeRequestApprovalResponse
{
    public required Guid Id { get; set; }
    public required Guid MergeRequestId { get; init; }
    public required Guid UserId { get; init; }
    public required bool Approved { get; init; }
}