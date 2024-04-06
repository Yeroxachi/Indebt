using Domain.Enums;

namespace Domain.Entities;

public class OptimizationRequest : Base
{
    public OptimizationRequest()
    {
        Approvals = new HashSet<OptimizationRequestApproval>();
    }
    
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public Guid InitiatorId { get; set; }
    public User Initiator { get; set; }
    public OptimizationStatus Status { get; set; }
    
    public ICollection<OptimizationRequestApproval> Approvals { get; set; }
}