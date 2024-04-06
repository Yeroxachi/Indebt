namespace Domain.Models;

public record EmailServiceOptions
{
    public string SenderName { get; init; }
    public string SenderAddress { get; init; }
    public string Password { get; init; }
    public string Host { get; init; }
    public int Port { get; init; }
    public bool SslState { get; init; }
}