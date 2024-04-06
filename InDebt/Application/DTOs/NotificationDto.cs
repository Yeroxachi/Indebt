namespace Application.DTOs;

public record NotificationDto
{
    public required Guid DebtId { get; init; }
    public required string Message { get; init; }
    public required DateTime TimeStamp { get; init; }
}