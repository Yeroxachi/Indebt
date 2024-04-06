namespace Domain.Entities;

public class RefreshToken : Base
{
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool IsValid { get; set; }
}