using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MergeController : BaseController
{
    private readonly IMergeRequestService _management;
    private readonly ILogger<MergeController> _logger;

    public MergeController(IMergeRequestService management, ILogger<MergeController> logger)
    {
        _management = management;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<MergeRequestResponse>>>> GetAll()
    {
        var response = await _management.GetAllAsync();
        return HandleRequest(response);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MergeRequestResponse>> GetById(Guid id)
    {
        var response = await _management.GetByIdAsync(id);
        return HandleRequest(response);
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<MergeRequestResponse>>> CreateMerge([FromBody] MergeRequestDto requestDto)
    {
        var response = await _management.CreateAsync(requestDto);
        return HandleRequest(response);
    }
    
    [HttpGet("Approvals")]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<MergeRequestApprovalResponse>>>> GetAllConfirmationWithStatusNoAction()
    {
        var response = await _management.GetAllReceivedAsync();
        return HandleRequest(response);
    }
    
    [HttpPut("{id:guid}/Accepted/{accepted:bool}")]
    public async Task<ActionResult<BaseResponse>> AcceptConfirmation([FromRoute]Guid id, [FromRoute]bool accepted)
    {
        var response = await _management.AcceptAsync(id, accepted);
        return HandleRequest(response);
    }
    
    [HttpPut("CompleteMerge/{id:guid}")]
    public async Task<ActionResult<BaseResponse>> CompleteMerge([FromRoute]Guid id)
    {
        var response = await _management.CompleteMerge(id);
        return HandleRequest(response);
    }
}