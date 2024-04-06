using Application.DTOs;
using Application.Responses;

namespace Application.Services;

public interface IAuthService 
{
    Task<BaseResponse> AuthenticateAsync(LoginDto dto);
    Task<BaseResponse> RegistrationAsync(RegistrationDto dto);
    Task<BaseResponse> ConfirmAccountAsync(EmailConfirmationDto dto);
    Task<BaseResponse> RefreshTokenAsync(string refreshToken);
    Task<BaseResponse> ResendEmailAsync(string email);
}