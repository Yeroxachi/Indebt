namespace Domain.Entities;

public class MergeRequestApproval : Base
{
    public Guid MergeRequestId { get; set; }
    public MergeRequest MergeRequest { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool Approved { get; set; }
}