using Application.DTOs;
using Application.Responses;
using Domain.Enums;

namespace Application.Services;

public interface ITransactionService
{
    Task<BaseResponse> GetAll(Guid? debtId, TransactionType? transactionType, PaginationDto paginationDto);
    Task<BaseResponse> GetById(Guid id);
    Task<BaseResponse> Create(TransactionDto dto);
    Task<BaseResponse> Update(Guid transactionId, TransactionDto dto);
    Task<BaseResponse> Accept(Guid transactionId);
    Task<BaseResponse> Delete(Guid transactionId);
}