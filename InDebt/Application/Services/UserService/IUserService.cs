using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface IUserService
{
    Task<BaseResponse> GetAllAsync(PaginationDto paginationDto);
    Task<BaseResponse> GetByIdAsync(Guid id);
    Task<BaseResponse> UpdateAsync(UserDto dto);
    Task<BaseResponse> DeleteAsync(Guid id);
}