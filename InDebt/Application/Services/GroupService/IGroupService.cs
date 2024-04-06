using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface IGroupService
{
    Task<BaseResponse> GetAllAsync(PaginationDto paginationDto);
    Task<BaseResponse> CreateAsync(GroupDto dto);
    Task<BaseResponse> GetByIdAsync(Guid groupId);
    Task<BaseResponse> UpdateAsync(Guid groupId, GroupDto dto);
    Task<BaseResponse> DeleteAsync(Guid groupId);
}