using Application.Responses;

namespace Application.Services;

public interface IBalanceService
{
    public Task<BaseResponse> GetTotal(Guid? groupId);
    public Task<BaseResponse> GetTotalIncome(Guid? groupId); //TODO should change to another word
    public Task<BaseResponse> GetTotalOutcome(Guid? groupId); //TODO should change to another word
}