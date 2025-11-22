// T042: TeamMembersController with GET endpoint
using Microsoft.AspNetCore.Mvc;
using TeamGoalTracker.Api.Data.Repositories;
using TeamGoalTracker.Api.Models;

namespace TeamGoalTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamMembersController : ControllerBase
{
    private readonly ITeamMemberRepository _repository;

    public TeamMembersController(ITeamMemberRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamMemberDto>>> GetAll()
    {
        var teamMembers = await _repository.GetAllAsync();
        var dtos = teamMembers.Select(tm => new TeamMemberDto(tm.Id, tm.Name, tm.CreatedAt));
        return Ok(dtos);
    }
}
