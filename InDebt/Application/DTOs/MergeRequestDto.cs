namespace Application.DTOs;

public record MergeRequestDto
{
    public required ICollection<Guid> GroupsId { get; init; }
    public required string NewName { get; init; }
    public required string Description { get; init; }
}