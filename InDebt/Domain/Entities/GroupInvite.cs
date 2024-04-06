using Domain.Enums;

namespace Domain.Entities;

public class GroupInvite : Base
{
    public Guid GroupId { set; get; }
    public Group Group { get; set; }
    public Guid InviterId { set; get; }
    public User Inviter { get; set; }
    public Guid InvitedId { set; get; }
    public User Invited { get;  set; }
    public InviteStatus InviteStatus { get; set; }
}