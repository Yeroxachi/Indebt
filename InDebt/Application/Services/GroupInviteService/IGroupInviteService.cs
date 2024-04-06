using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface IGroupInviteService
{
    Task<BaseResponse> SendAsync(GroupInviteDto dto);
    Task<BaseResponse> AcceptAsync(Guid inviteId);
    Task<BaseResponse> GetAllReceivedAsync();
}