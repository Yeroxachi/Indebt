using Domain.Enums;

namespace Domain.Entities;

public class MergeRequest : Base
{
    public MergeRequest()
    {
        Groups = new HashSet<MergeRequestGroup>();
        Approvals = new HashSet<MergeRequestApproval>();
    }
    
    public Guid InitiatorId { get; set; }
    public User Initiator { get; set; }
    public string NewGroupName { get; set; }
    public string NewDescription { get; set; }
    
    public MergeStatus MergeStatus { get; set; }
    public ICollection<MergeRequestGroup> Groups { get; set; }
    public ICollection<MergeRequestApproval> Approvals { get; set; }
    public bool ReadyToMerge
    {
        get
        {
            if (Groups.Any(x => x.Group == null))
                throw new ArgumentNullException();

            var groupMembers = Groups.SelectMany(x => x.Group.Users).Select(x=> x.User.Id).ToHashSet();
            groupMembers.Remove(Initiator.Id);
            return groupMembers.All(x => Approvals.Any(y => y.UserId == Id && y.Approved));
        }
    }
}