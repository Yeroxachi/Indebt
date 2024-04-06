namespace Application.Responses;

public record OptimizationResponse
{
    public required Guid Id { get; init; }
    public required Guid GroupId { get; init; }
    public required Guid InitiatorId { get; init; }
    public required string Status { get; init; }
}