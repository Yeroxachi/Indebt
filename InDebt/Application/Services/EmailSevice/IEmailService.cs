using Application.DTOs;

namespace Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailDto dto);
}