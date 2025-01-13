using Dapper;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HomeAssistant.PostgreSql.Repositories;

public class HourRepository : IHourRepository
{
    private string ConnectionString { get; }
    
    public HourRepository(IConfiguration configuration)
    {
        ConnectionString = configuration["Postgresql:ConnectionString"];
    }
    
    public async Task<IHour> AddHourAsync(IHour hour)
    {
        string sql = "INSERT INTO hour(date, start_at, end_at) VALUES(@Date, @StartAt, @EndAt) RETURNING id";
        await using var con = new NpgsqlConnection(ConnectionString);
        hour.Id = await con.QueryFirstAsync<int>(sql, hour);
        return hour;
    }

    public async Task<IEnumerable<IHour>> GetHoursByDateAsync(DateTime date)
    {
        string sql = "SELECT * FROM hour WHERE date = @Date::date";
        await using var con = new NpgsqlConnection(ConnectionString);
        return  await con.QueryAsync<Hour>(sql, new { Date = date});
    }
}