using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthsController : BaseController
{
    private readonly IAuthService _management;
    private readonly ILogger<AuthsController> _logger;

    public AuthsController(IAuthService management, ILogger<AuthsController> logger)
    {
        _management = management;
        _logger = logger;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<BaseResponse<TokenResponse>>> Authenticate([FromBody]LoginDto dto)
    {
        var response = await _management.AuthenticateAsync(dto);
        return HandleRequest(response);
    }
    
    [HttpPost("RefreshToken")]
    public async Task<ActionResult<BaseResponse<TokenResponse>>> RefreshToken([FromBody]RefreshTokenDto refreshToken)
    {
        var response = await _management.RefreshTokenAsync(refreshToken.RefreshToken);
        return HandleRequest(response);
    }
    
    [HttpPost("Registration")]
    public async Task<ActionResult<BaseResponse<UserResponse>>> Registration([FromBody]RegistrationDto dto)
    {
        var response = await _management.RegistrationAsync(dto);
        return HandleRequest(response);
    }
    
    [HttpPost("Email/Confirm")]
    public async Task<ActionResult<BaseResponse<TokenResponse>>> ConfirmEmail([FromBody] EmailConfirmationDto dto)
    {
        var response = await _management.ConfirmAccountAsync(dto);
        return HandleRequest(response);
    }

    [HttpPost("Email/Resend/{email}")]
    public async Task<ActionResult<BaseResponse>> ResendEmail([FromRoute]string email)
    {
        var response = await _management.ResendEmailAsync(email);
        return HandleRequest(response);
    }
}