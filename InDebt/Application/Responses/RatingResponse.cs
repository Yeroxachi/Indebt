namespace Application.Responses;

public record RatingResponse
{
    public required Guid UserId { get; init; }
    public required double Rating { get; init; }
}