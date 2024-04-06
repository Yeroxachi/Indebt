using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseController
{
    private readonly IUserService _management;
    private readonly ILogger<AuthsController> _logger;

    public UsersController(IUserService management, ILogger<AuthsController> logger)
    {
        _management = management;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<UserResponse>>> GetAllUsers([FromQuery] PaginationDto paginationDto)
    {
        var response = await _management.GetAllAsync(paginationDto); 
        return HandleRequest(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<UserResponse>>> GetUserById([FromRoute]Guid id)
    {
        var response = await _management.GetByIdAsync(id);
        return HandleRequest(response);
    }

    [HttpPut]
    public async Task<ActionResult<BaseResponse>> UpdateUser([FromBody]UserDto dto)
    {
        var response = await _management.UpdateAsync(dto);
        return HandleRequest(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse>> DeleteUser([FromRoute]Guid id)
    {
        var response = await _management.DeleteAsync(id);
        return HandleRequest(response);
    }
}
