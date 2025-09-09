namespace RL.Backend.Models;

public class UpsertAssignmentsDto
{
    public int PlanId { get; set; }
    public int ProcedureId { get; set; }
    public IEnumerable<int> UserIds { get; set; } = Array.Empty<int>();
}