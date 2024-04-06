using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface ICurrencyService
{
    Task<BaseResponse> GetAll(PaginationDto paginationDto);
}