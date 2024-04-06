using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InvitesController : BaseController
{
    private readonly IGroupInviteService _management;
    private readonly ILogger<AuthsController> _logger;

    public InvitesController(IGroupInviteService management, ILogger<AuthsController> logger)
    {
        _management = management;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<GroupInviteResponse>>> SendInvite([FromBody]GroupInviteDto dto)
    {
        var response = await _management.SendAsync(dto);
        return HandleRequest(response);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BaseResponse<GroupInviteResponse>>> AcceptInvite([FromRoute] Guid id)
    {
        var response = await _management.AcceptAsync(id);
        return HandleRequest(response);
    }
    
    [HttpGet]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<GroupInviteResponse>>>> GetAllInvites()
    {
        var response = await _management.GetAllReceivedAsync();
        return HandleRequest(response);
    }
}