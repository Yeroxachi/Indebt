using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record GroupDto
{
    [MaxLength(100),MinLength(3)]
    public required string Name { get; init; }
    [MaxLength(500),MinLength(3)]
    public required string Description { get; init; }
}