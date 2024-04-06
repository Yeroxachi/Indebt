namespace Domain.Entities;

public class UserGroup : Base
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
}