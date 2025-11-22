// T043: StatsController with GET endpoint
using Microsoft.AspNetCore.Mvc;
using TeamGoalTracker.Api.Data.Repositories;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Api.Services;

namespace TeamGoalTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IGoalRepository _goalRepository;
    private readonly IMoodRepository _moodRepository;
    private readonly IStatsService _statsService;

    public StatsController(
        IGoalRepository goalRepository,
        IMoodRepository moodRepository,
        IStatsService statsService)
    {
        _goalRepository = goalRepository;
        _moodRepository = moodRepository;
        _statsService = statsService;
    }

    [HttpGet]
    public async Task<ActionResult<TeamStatsDto>> GetStats([FromQuery] DateTime? date = null)
    {
        var targetDate = date ?? DateTime.Today;

        var goals = await _goalRepository.GetByDateAsync(targetDate);
        var moods = await _moodRepository.GetByDateAsync(targetDate);

        var stats = _statsService.CalculateStats(goals, moods);

        return Ok(stats);
    }
}
