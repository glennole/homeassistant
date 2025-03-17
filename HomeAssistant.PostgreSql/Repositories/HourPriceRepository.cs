using Dapper;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HomeAssistant.PostgreSql.Repositories;

public class HourPriceRepository : IHourPriceRepository
{
    private string ConnectionString { get; }
    
    public HourPriceRepository(IConfiguration configuration)
    {
        ConnectionString = configuration["Postgresql:ConnectionString"];
    }
    public async Task<IHourPrice> AddHourPriceAsync(IHourPrice hourPrice)
    {
        string sql = "INSERT INTO hour_price(hour_id, price) VALUES(@HourId, @Price) RETURNING id";
        await using var con = new NpgsqlConnection(ConnectionString);
        hourPrice.Id = await con.QueryFirstAsync<int>(sql, hourPrice);
        return hourPrice;
    }
}