// T018: Mood entity and MoodType enum
namespace TeamGoalTracker.Core.Entities;

public class Mood
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    public MoodType MoodType { get; set; }
    public DateTime Date { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum MoodType
{
    VeryHappy = 1,
    Happy = 2,
    Neutral = 3,
    Sad = 4,
    Stressed = 5
}
