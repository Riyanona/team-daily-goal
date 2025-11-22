// T041: DashboardService with optimized data loading
using TeamGoalTracker.Api.Data.Repositories;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardDataAsync(DateTime date);
}

public class DashboardService : IDashboardService
{
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IGoalRepository _goalRepository;
    private readonly IMoodRepository _moodRepository;
    private readonly IStatsService _statsService;

    public DashboardService(
        ITeamMemberRepository teamMemberRepository,
        IGoalRepository goalRepository,
        IMoodRepository moodRepository,
        IStatsService statsService)
    {
        _teamMemberRepository = teamMemberRepository;
        _goalRepository = goalRepository;
        _moodRepository = moodRepository;
        _statsService = statsService;
    }

    public async Task<DashboardResponse> GetDashboardDataAsync(DateTime date)
    {
        // Parallel queries for better performance
        var teamMembersTask = _teamMemberRepository.GetAllAsync();
        var goalsTask = _goalRepository.GetByDateAsync(date);
        var moodsTask = _moodRepository.GetByDateAsync(date);

        await Task.WhenAll(teamMembersTask, goalsTask, moodsTask);

        var teamMembers = (await teamMembersTask).ToList();
        var goals = (await goalsTask).ToList();
        var moods = (await moodsTask).ToList();

        // Map goals and moods to team members
        var teamMembersWithDetails = teamMembers.Select(tm =>
        {
            var memberGoals = goals.Where(g => g.TeamMemberId == tm.Id)
                .Select(g => new GoalDto(g.Id, g.TeamMemberId, g.Description, g.IsCompleted, g.Date, g.CreatedAt))
                .ToList();

            var memberMood = moods.FirstOrDefault(m => m.TeamMemberId == tm.Id);
            var moodDto = memberMood != null
                ? new MoodDto(memberMood.Id, memberMood.TeamMemberId, memberMood.MoodType, memberMood.Date, memberMood.UpdatedAt)
                : null;

            return new TeamMemberWithDetailsDto(
                tm.Id,
                tm.Name,
                tm.CreatedAt,
                memberGoals,
                moodDto
            );
        }).ToList();

        var stats = _statsService.CalculateStats(goals, moods);

        return new DashboardResponse(teamMembersWithDetails, stats, date);
    }
}
