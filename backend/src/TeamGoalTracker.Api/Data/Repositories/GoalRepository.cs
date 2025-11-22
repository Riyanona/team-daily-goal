// T038: GoalRepository with GetByDateAsync
using Dapper;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Data.Repositories;

public interface IGoalRepository
{
    Task<IEnumerable<Goal>> GetByDateAsync(DateTime date);
    Task<int> InsertAsync(Goal goal);
    Task<bool> UpdateCompletionStatusAsync(int goalId, bool isCompleted);
    Task<Goal?> GetByIdAsync(int goalId);
}

public class GoalRepository : IGoalRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GoalRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Goal>> GetByDateAsync(DateTime date)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT Id, TeamMemberId, Description, IsCompleted, Date, CreatedAt
            FROM Goals
            WHERE Date = @Date
            ORDER BY CreatedAt";

        return await connection.QueryAsync<Goal>(sql, new { Date = date.Date });
    }

    // T058: InsertAsync method
    public async Task<int> InsertAsync(Goal goal)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Goals (TeamMemberId, Description, IsCompleted, Date, CreatedAt)
            VALUES (@TeamMemberId, @Description, @IsCompleted, @Date, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        return await connection.ExecuteScalarAsync<int>(sql, goal);
    }

    // T091: UpdateCompletionStatusAsync method
    public async Task<bool> UpdateCompletionStatusAsync(int goalId, bool isCompleted)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Goals
            SET IsCompleted = @IsCompleted
            WHERE Id = @GoalId";

        var rowsAffected = await connection.ExecuteAsync(sql, new { GoalId = goalId, IsCompleted = isCompleted });
        return rowsAffected > 0;
    }

    public async Task<Goal?> GetByIdAsync(int goalId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT Id, TeamMemberId, Description, IsCompleted, Date, CreatedAt
            FROM Goals
            WHERE Id = @GoalId";

        return await connection.QuerySingleOrDefaultAsync<Goal>(sql, new { GoalId = goalId });
    }
}
