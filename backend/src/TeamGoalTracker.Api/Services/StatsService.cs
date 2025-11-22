// T040: StatsService with calculation methods
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Services;

public interface IStatsService
{
    TeamStatsDto CalculateStats(IEnumerable<Goal> goals, IEnumerable<Mood> moods);
}

public class StatsService : IStatsService
{
    public TeamStatsDto CalculateStats(IEnumerable<Goal> goals, IEnumerable<Mood> moods)
    {
        var goalsList = goals.ToList();
        var totalGoals = goalsList.Count;
        var completedGoals = goalsList.Count(g => g.IsCompleted);
        var completionPercentage = totalGoals > 0 ? (double)completedGoals / totalGoals * 100 : 0;

        var moodDistribution = new Dictionary<MoodType, int>
        {
            { MoodType.VeryHappy, 0 },
            { MoodType.Happy, 0 },
            { MoodType.Neutral, 0 },
            { MoodType.Sad, 0 },
            { MoodType.Stressed, 0 }
        };

        foreach (var mood in moods)
        {
            if (moodDistribution.ContainsKey(mood.MoodType))
            {
                moodDistribution[mood.MoodType]++;
            }
        }

        return new TeamStatsDto(
            Math.Round(completionPercentage, 2),
            moodDistribution,
            totalGoals,
            completedGoals
        );
    }
}
