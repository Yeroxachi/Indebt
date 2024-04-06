using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface INotificationService
{
    Task<BaseResponse> GetAllUnreadAsync(PaginationDto paginationDto);
    Task<BaseResponse> MarkAsReadAsync(Guid id);
    Task<BaseResponse> CreateAsync(NotificationDto dto);
    Task<BaseResponse> DeleteAsync(Guid id);
}