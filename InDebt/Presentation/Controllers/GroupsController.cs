using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupsController : BaseController
{
    private readonly IGroupService _management;
    private readonly ILogger<AuthsController> _logger;

    public GroupsController(IGroupService management, ILogger<AuthsController> logger)
    {
        _management = management;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginationResponse<GroupResponse>>>> GetAllGroups([FromQuery]PaginationDto paginationDto)
    {
        var response = await _management.GetAllAsync(paginationDto);
        return HandleRequest(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<GroupResponse>>> GetGroupById([FromRoute]Guid id)
    {
        var response = await _management.GetByIdAsync(id);
        return HandleRequest(response);
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<GroupResponse>>> CreateGroup([FromBody]GroupDto dto)
    {
        var response = await _management.CreateAsync(dto);
        return HandleRequest(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BaseResponse<GroupResponse>>> UpdateGroup([FromRoute]Guid id, [FromBody]GroupDto dto)
    {
        var response = await _management.UpdateAsync(id, dto);
        return HandleRequest(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse>> DeleteGroup([FromRoute]Guid id)
    {
        var response = await _management.DeleteAsync(id);
        return HandleRequest(response);
    }
}