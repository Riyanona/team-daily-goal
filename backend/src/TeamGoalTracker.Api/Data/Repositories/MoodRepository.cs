// T039: MoodRepository with GetByDateAsync
using Dapper;
using TeamGoalTracker.Core.Entities;

namespace TeamGoalTracker.Api.Data.Repositories;

public interface IMoodRepository
{
    Task<IEnumerable<Mood>> GetByDateAsync(DateTime date);
    Task UpsertAsync(Mood mood);
}

public class MoodRepository : IMoodRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MoodRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Mood>> GetByDateAsync(DateTime date)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT Id, TeamMemberId, MoodType, Date, UpdatedAt
            FROM Moods
            WHERE Date = @Date";

        return await connection.QueryAsync<Mood>(sql, new { Date = date.Date });
    }

    // T075: UpsertAsync method (INSERT or UPDATE)
    public async Task UpsertAsync(Mood mood)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            MERGE INTO Moods AS target
            USING (SELECT @TeamMemberId AS TeamMemberId, @Date AS Date) AS source
            ON target.TeamMemberId = source.TeamMemberId AND target.Date = source.Date
            WHEN MATCHED THEN
                UPDATE SET MoodType = @MoodType, UpdatedAt = @UpdatedAt
            WHEN NOT MATCHED THEN
                INSERT (TeamMemberId, MoodType, Date, UpdatedAt)
                VALUES (@TeamMemberId, @MoodType, @Date, @UpdatedAt);";

        await connection.ExecuteAsync(sql, mood);
    }
}
