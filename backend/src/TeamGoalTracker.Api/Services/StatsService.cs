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

        var moodDistribution = new Dictionary<int, int>
        {
            { 1, 0 }, // VeryHappy
            { 2, 0 }, // Happy
            { 3, 0 }, // Neutral
            { 4, 0 }, // Sad
            { 5, 0 }  // Stressed
        };

        foreach (var mood in moods)
        {
            var moodTypeValue = (int)mood.MoodType;
            if (moodDistribution.ContainsKey(moodTypeValue))
            {
                moodDistribution[moodTypeValue]++;
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
