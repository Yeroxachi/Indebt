using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface IExchangeRateService
{
    Task<BaseResponse<ExchangeRateResponse>> CalculateExchangeRateAsync(ExchangeRateDto dto);
}