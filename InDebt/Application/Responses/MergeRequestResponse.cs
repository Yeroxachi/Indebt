namespace Application.Responses;

public record MergeRequestResponse
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string NewGroupName { get; init; }
    public required string NewDescription { get; init; }
    public required string MergeStatus { get; init; }
    public required ICollection<Guid> MergeGroups { get; set; }
}