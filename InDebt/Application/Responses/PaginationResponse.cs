namespace Application.Responses;

public record PaginationResponse<T>(int TotalCount, ICollection<T> Data);