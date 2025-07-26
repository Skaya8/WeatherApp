using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Repositories
{
    public class ChangeLogRepository : IChangeLogRepository
    {
        private readonly string _connectionString;

        public ChangeLogRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<WeatherSearchChangeLog>> GetWeatherSearchChangesAsync(int weatherSearchId)
        {
            var logs = new List<WeatherSearchChangeLog>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetWeatherSearchChanges", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@WeatherSearchId", weatherSearchId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                logs.Add(new WeatherSearchChangeLog
                {
                    ChangeDate = reader.GetDateTime(reader.GetOrdinal("ChangeDate")),
                    ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                    OldValue = reader["OldValue"]?.ToString(),
                    NewValue = reader["NewValue"]?.ToString(),
                    Username = reader["Username"]?.ToString()
                });
            }
            return logs;
        }
    }
} 