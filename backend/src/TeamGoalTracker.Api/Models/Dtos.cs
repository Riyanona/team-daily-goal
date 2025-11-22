// T025: DTOs for API requests and responses
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Models;

// Request DTOs
public record CreateGoalRequest(int TeamMemberId, string Description, DateTime? Date);
public record UpdateMoodRequest(int TeamMemberId, MoodType MoodType);

// Response DTOs
public record TeamMemberDto(
    int Id,
    string Name,
    DateTime CreatedAt
);

public record GoalDto(
    int Id,
    int TeamMemberId,
    string Description,
    bool IsCompleted,
    DateTime Date,
    DateTime CreatedAt
);

public record MoodDto(
    int Id,
    int TeamMemberId,
    MoodType MoodType,
    DateTime Date,
    DateTime UpdatedAt
);

public record TeamStatsDto(
    double CompletionPercentage,
    Dictionary<int, int> MoodDistribution,
    int TotalGoals,
    int CompletedGoals
);

public record TeamMemberWithDetailsDto(
    int Id,
    string Name,
    DateTime CreatedAt,
    List<GoalDto> Goals,
    MoodDto? CurrentMood
);

public record DashboardResponse(
    List<TeamMemberWithDetailsDto> TeamMembers,
    TeamStatsDto Stats,
    DateTime Date
);
