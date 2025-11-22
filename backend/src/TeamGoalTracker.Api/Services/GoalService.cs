// T060: GoalService with CreateGoalAsync method
using System.Data.SqlTypes;
using TeamGoalTracker.Api.Data.Repositories;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Services;

public interface IGoalService
{
    Task<GoalDto> CreateGoalAsync(CreateGoalRequest request);
    Task<GoalDto> CompleteGoalAsync(int goalId);
}

public class GoalService : IGoalService
{
    private readonly IGoalRepository _goalRepository;

    public GoalService(IGoalRepository goalRepository)
    {
        _goalRepository = goalRepository;
    }

    public async Task<GoalDto> CreateGoalAsync(CreateGoalRequest request)
    {
        var goalDate = request.Date?.Date ?? DateTime.UtcNow.Date;
        var safeDate = goalDate < SqlDateTime.MinValue.Value ? SqlDateTime.MinValue.Value : goalDate;
        var safeCreatedAt = DateTime.UtcNow < SqlDateTime.MinValue.Value ? SqlDateTime.MinValue.Value : DateTime.UtcNow;

        var goal = new Goal
        {
            TeamMemberId = request.TeamMemberId,
            Description = request.Description,
            IsCompleted = false,
            Date = safeDate,
            CreatedAt = safeCreatedAt
        };

        var id = await _goalRepository.InsertAsync(goal);
        goal.Id = id;

        return new GoalDto(
            goal.Id,
            goal.TeamMemberId,
            goal.Description,
            goal.IsCompleted,
            goal.Date,
            goal.CreatedAt);
    }

    // T092: CompleteGoalAsync method
    public async Task<GoalDto> CompleteGoalAsync(int goalId)
    {
        var goal = await _goalRepository.GetByIdAsync(goalId);
        if (goal == null)
        {
            throw new ArgumentException($"Goal with ID {goalId} not found");
        }

        var updated = await _goalRepository.UpdateCompletionStatusAsync(goalId, true);
        if (!updated)
        {
            throw new InvalidOperationException($"Failed to update goal {goalId}");
        }

        goal.IsCompleted = true;

        return new GoalDto(
            goal.Id,
            goal.TeamMemberId,
            goal.Description,
            goal.IsCompleted,
            goal.Date,
            goal.CreatedAt);
    }
}
