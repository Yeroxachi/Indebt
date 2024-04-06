namespace Application.DTOs;

public record PaginationDto
{
    public required int PageNumber { get; init;}
    public required int PageSize { get; init; }
    
    public int SkipCount()
    {
        return (PageNumber - 1) * PageSize;
    }
}