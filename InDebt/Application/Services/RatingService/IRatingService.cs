using Application.Responses;

namespace Application.Services;

public interface IRatingService
{
    Task<BaseResponse> GetRatingOfUserByIdAsync(Guid userId);
    Task<BaseResponse> GetRatingOfUsersByGroupIdAsync(Guid groupId);
}