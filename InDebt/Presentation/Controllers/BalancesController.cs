using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

[Authorize]
public class BalancesController : BaseController
{
    private readonly IBalanceService _balanceService;
    private readonly ILogger<BaseController> _logger;

    public BalancesController(IBalanceService balanceService, ILogger<BaseController> logger)
    {
        _balanceService = balanceService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<BaseResponse<BalanceResponse>>> GetTotal([FromQuery]Guid? groupId)
    {
        var response = await _balanceService.GetTotal(groupId);
        return HandleRequest(response);
    }
    
    [HttpGet("Incoming")]
    public async Task<ActionResult<BaseResponse<BalanceResponse>>> GetTotalIncome([FromQuery]Guid? groupId)
    {
        var response = await _balanceService.GetTotalIncome(groupId);
        return HandleRequest(response);
    }
    
    [HttpGet("Outgoing")]
    public async Task<ActionResult<BaseResponse<BalanceResponse>>> GetTotalOutcome([FromQuery]Guid? groupId)
    {
        var response = await _balanceService.GetTotalOutcome(groupId);
        return HandleRequest(response);
    }
}