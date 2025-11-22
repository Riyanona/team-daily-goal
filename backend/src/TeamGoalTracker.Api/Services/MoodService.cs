// T077: MoodService with UpdateMoodAsync method
using TeamGoalTracker.Api.Data.Repositories;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Services;

public interface IMoodService
{
    Task<MoodDto> UpdateMoodAsync(UpdateMoodRequest request);
}

public class MoodService : IMoodService
{
    private readonly IMoodRepository _moodRepository;

    public MoodService(IMoodRepository moodRepository)
    {
        _moodRepository = moodRepository;
    }

    public async Task<MoodDto> UpdateMoodAsync(UpdateMoodRequest request)
    {
        var mood = new Mood
        {
            TeamMemberId = request.TeamMemberId,
            MoodType = request.MoodType,
            Date = DateTime.UtcNow.Date,
            UpdatedAt = DateTime.UtcNow
        };

        await _moodRepository.UpsertAsync(mood);

        return new MoodDto(
            0, // ID not returned from upsert
            mood.TeamMemberId,
            mood.MoodType,
            mood.Date,
            mood.UpdatedAt);
    }
}
