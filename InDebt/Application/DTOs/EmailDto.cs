using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record EmailDto
{
    public required string Subject { get; init; }
    public required string ReceiverName { get; init; }
    [EmailAddress]
    public required string ReceiverEmail { get; init; }
    public required string Message { get; init; }
}