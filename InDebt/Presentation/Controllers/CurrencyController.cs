using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

public class CurrencyController : BaseController
{
    private readonly ICurrencyService _management;

    public CurrencyController(ICurrencyService management)
    {
        _management = management;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<CurrencyResponse>>>> Get([FromQuery] PaginationDto paginationDto)
    {
        var response = await _management.GetAll(paginationDto);
        return HandleRequest(response);
    }
}
