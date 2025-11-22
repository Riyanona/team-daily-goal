// T037: TeamMemberRepository with GetAllAsync
using Dapper;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Data.Repositories;

public interface ITeamMemberRepository
{
    Task<IEnumerable<TeamMember>> GetAllAsync();
}

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TeamMemberRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TeamMember>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT Id, Name, CreatedAt
            FROM TeamMembers
            ORDER BY Name";

        return await connection.QueryAsync<TeamMember>(sql);
    }
}
