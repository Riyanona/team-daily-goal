// T017: Goal entity
namespace TeamGoalTracker.Core.Entities;

public class Goal
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
}
