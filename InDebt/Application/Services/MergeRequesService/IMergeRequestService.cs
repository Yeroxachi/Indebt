using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface IMergeRequestService
{
    Task<BaseResponse> CreateAsync(MergeRequestDto requestDto);
    Task<BaseResponse> GetAllReceivedAsync();
    Task<BaseResponse> AcceptAsync(Guid confirmationId, bool accepted);
    Task<BaseResponse> CompleteMerge (Guid id);
    Task<BaseResponse> GetAllAsync ();
    Task<BaseResponse> GetByIdAsync (Guid id);
    Task<BaseResponse> UpdateAsync (MergeRequestDto dto, Guid id);
}