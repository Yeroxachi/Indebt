namespace Application.Responses;

public record OptimizationApprovalResponse
{
    public required Guid Id { get; init; }
    public required Guid OptimizationRequestId { get; init; }
    public required Guid UserId { get; init; }
    public required bool Approved { get; init; }
}