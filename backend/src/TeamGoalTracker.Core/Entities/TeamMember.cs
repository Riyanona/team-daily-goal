// T016: TeamMember entity
namespace TeamGoalTracker.Core.Entities;

public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation properties (populated by Dapper multi-mapping)
    public List<Goal> Goals { get; set; } = new();
    public Mood? CurrentMood { get; set; }
}
