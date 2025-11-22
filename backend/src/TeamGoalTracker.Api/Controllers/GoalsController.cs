// T061-T062: GoalsController with POST endpoint and error handling
using Microsoft.AspNetCore.Mvc;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Api.Services;

namespace TeamGoalTracker.Api.Controllers;

[ApiController]
[Route("api/goals")]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;
    private readonly ILogger<GoalsController> _logger;

    public GoalsController(IGoalService goalService, ILogger<GoalsController> logger)
    {
        _goalService = goalService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<GoalDto>> CreateGoal([FromBody] CreateGoalRequest request)
    {
        try
        {
            var goal = await _goalService.CreateGoalAsync(request);
            return CreatedAtAction(nameof(CreateGoal), new { id = goal.Id }, goal);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating goal");
            return BadRequest(new { error = ex.Message });
        }
    }

    // T093-T094: PATCH endpoint to complete goal with error handling
    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<GoalDto>> CompleteGoal(int id)
    {
        try
        {
            var goal = await _goalService.CompleteGoalAsync(id);
            return Ok(goal);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Goal not found: {GoalId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to complete goal: {GoalId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
