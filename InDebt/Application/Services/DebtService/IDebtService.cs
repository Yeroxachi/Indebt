using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface IDebtService
{
    Task<BaseResponse> GetAll(bool? completed, bool? isBorrower, PaginationDto dto);
    Task<BaseResponse> GetById(Guid debtId);
    Task<BaseResponse> Create(DebtDto dto);
    Task<BaseResponse> Update(Guid debtId, DebtDto dto);
    Task<BaseResponse> Accept(Guid debtId);
    Task<BaseResponse> Delete(Guid debtId);
}