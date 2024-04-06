namespace Application.Responses;

public record NotificationResponse
{
    public required Guid Id { get; init; }
    public required Guid DebtId { get; init; }
    public required string Message { get; init; }
    public required DateTime TimeStamp { get; init; }
    public required bool IsRead { get; init; }
}