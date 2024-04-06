using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

public class OptimizationController : BaseController
{
    private readonly IDebtOptimizationService _optimizationService;
    private readonly ILogger<OptimizationController> _logger;

    public OptimizationController(IDebtOptimizationService optimizationService, ILogger<OptimizationController> logger)
    {
        _optimizationService = optimizationService;
        _logger = logger;
    }
    
    
    [HttpGet]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<OptimizationResponse>>>> GetAll()
    {
        var response = await _optimizationService.GetAllAsync();
        return HandleRequest(response);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<OptimizationResponse>>> GetById([FromRoute]Guid id)
    {
        var response = await _optimizationService.GetByIdAsync(id);
        return HandleRequest(response);
    }
    
    [HttpGet("Approvals")]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<OptimizationApprovalResponse>>>> GetAllReceivedApprovals()
    {
        var response = await _optimizationService.GetAllReceivedApprovalsAsync();
        return HandleRequest(response);
    }
    
    [HttpPost("{groupId:guid}")]
    public async Task<ActionResult<BaseResponse<OptimizationResponse>>> Create([FromRoute]Guid groupId)
    {
        var response = await _optimizationService.CreateAsync(groupId);
        return HandleRequest(response);
    }
    
    [HttpPatch("Approvals/{id:guid}")]
    public async Task<ActionResult<BaseResponse<OptimizationApprovalResponse>>> AcceptApproval([FromRoute]Guid id)
    {
        var response = await _optimizationService.AcceptApprovalAsync(id);
        return HandleRequest(response);
    }
    
    [HttpPost("{id:guid}/Optimize")]
    public async Task<ActionResult<BaseResponse<OptimizationApprovalResponse>>> Optimization([FromRoute]Guid id)
    {
        var response = await _optimizationService.OptimizeAsync(id);
        return HandleRequest(response);
    }
}