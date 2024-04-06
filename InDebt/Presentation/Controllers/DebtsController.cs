using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

[Authorize]
public class DebtsController : BaseController
{
    private readonly IDebtService _management;
    private readonly ILogger<DebtsController> _logger;

    public DebtsController(IDebtService management, ILogger<DebtsController> logger)
    {
        _management = management;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginationResponse<DebtResponse>>>> GetAll([FromQuery]bool? completed, [FromQuery]bool? isBorrower, [FromQuery]PaginationDto paginationDto)
    {
        var response = await _management.GetAll(completed, isBorrower, paginationDto);
        return HandleRequest(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<DebtResponse>>> GetById([FromRoute]Guid id)
    {
        var response = await _management.GetById(id);
        return HandleRequest(response);
    }
    
    [HttpPost]
    public async Task<ActionResult<BaseResponse<DebtResponse>>> CreateDebts([FromBody]DebtDto dto)
    {
        var response = await _management.Create(dto);
        return HandleRequest(response);
    }

    [HttpPut("{id:guid}/accept")]
    public async Task<ActionResult<BaseResponse<DebtResponse>>> AcceptDebt([FromRoute]Guid id)
    {
        var response = await _management.Accept(id);
        return HandleRequest(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse>> DeleteDebt([FromRoute]Guid id)
    {
        var response = await _management.Delete(id);
        return HandleRequest(response);
    }
}
