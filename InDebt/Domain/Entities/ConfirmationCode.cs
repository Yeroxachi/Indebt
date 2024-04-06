using Domain.Enums;

namespace Domain.Entities;

public class ConfirmationCode : Base
{
    public string Code { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public ConfirmationType Type { get; set; }
}