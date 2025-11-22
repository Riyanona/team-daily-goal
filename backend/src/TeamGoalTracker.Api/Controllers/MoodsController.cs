// T078-T079: MoodsController with PUT endpoint and error handling
using Microsoft.AspNetCore.Mvc;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Api.Services;

namespace TeamGoalTracker.Api.Controllers;

[ApiController]
[Route("api/moods")]
public class MoodsController : ControllerBase
{
    private readonly IMoodService _moodService;
    private readonly ILogger<MoodsController> _logger;

    public MoodsController(IMoodService moodService, ILogger<MoodsController> logger)
    {
        _moodService = moodService;
        _logger = logger;
    }

    [HttpPut]
    public async Task<ActionResult<MoodDto>> UpdateMood([FromBody] UpdateMoodRequest request)
    {
        try
        {
            var mood = await _moodService.UpdateMoodAsync(request);
            return Ok(mood);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating mood");
            return BadRequest(new { error = ex.Message });
        }
    }
}
