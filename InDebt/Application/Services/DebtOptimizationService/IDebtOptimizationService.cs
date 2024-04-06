using Application.Responses;

namespace Application.Services;

public interface IDebtOptimizationService
{
    public Task<BaseResponse> GetAllReceivedApprovalsAsync();
    public Task<BaseResponse> GetAllAsync();
    public Task<BaseResponse> GetByIdAsync(Guid id);
    public Task<BaseResponse> CreateAsync(Guid groupId);
    public Task<BaseResponse> AcceptApprovalAsync(Guid id);
    public Task<BaseResponse> OptimizeAsync(Guid id);
}