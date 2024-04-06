namespace Domain.Entities;

public class OptimizationRequestApproval : Base
{
    public Guid OptimizationRequestId { get; set; }
    public OptimizationRequest OptimizationRequest { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool Approved { get; set; }
}