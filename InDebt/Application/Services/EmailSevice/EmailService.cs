using Application.DTOs;
using Domain.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Services;

public class EmailService: IEmailService
{
    private readonly IOptions<EmailServiceOptions> _options;

    public EmailService(IOptions<EmailServiceOptions> options)
    {
        _options = options;
    }
    public async Task SendEmailAsync(EmailDto dto)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_options.Value.SenderName, _options.Value.SenderAddress));
        emailMessage.To.Add(new MailboxAddress(dto.ReceiverName, dto.ReceiverEmail));
        emailMessage.Subject = dto.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = dto.Message
        };
        
        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Value.Host, _options.Value.Port);
        await client.AuthenticateAsync(_options.Value.SenderAddress, _options.Value.Password);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}