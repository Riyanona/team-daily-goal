// T044: DashboardController with GET endpoint
using Microsoft.AspNetCore.Mvc;
using TeamGoalTracker.Api.Models;
using TeamGoalTracker.Api.Services;

namespace TeamGoalTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> GetDashboard([FromQuery] DateTime? date = null)
    {
        var targetDate = date ?? DateTime.Today;
        var dashboard = await _dashboardService.GetDashboardDataAsync(targetDate);
        return Ok(dashboard);
    }
}
