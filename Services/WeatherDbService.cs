using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherDbService
    {
        private readonly string _connectionString;

        public WeatherDbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<int?> ValidateUserAsync(string username, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ValidateUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

            await conn.OpenAsync();
            var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(0);
            }
            return null;
        }

        public async Task SaveWeatherSearchAsync(int userId, string city, int humidity, double tempMin, double tempMax, DateTime searchDate,
    string? condition, double? currentTemp, double? windSpeed, int? windDeg)
{
    using var conn = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
    using var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_InsertWeatherSearch", conn)
    {
        CommandType = System.Data.CommandType.StoredProcedure
    };
    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@City", city);
    cmd.Parameters.AddWithValue("@Humidity", humidity);
    cmd.Parameters.AddWithValue("@TempMin", tempMin);
    cmd.Parameters.AddWithValue("@TempMax", tempMax);
    cmd.Parameters.AddWithValue("@SearchDate", searchDate);
    cmd.Parameters.AddWithValue("@Condition", (object?)condition ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@CurrentTemp", (object?)currentTemp ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@WindSpeed", (object?)windSpeed ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@WindDeg", (object?)windDeg ?? DBNull.Value);

    await conn.OpenAsync();
    await cmd.ExecuteNonQueryAsync();
}

        public async Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var results = new List<WeatherSearchResult>();
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            using var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_GetWeatherSearches", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object?)city ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Username", (object?)username ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new WeatherSearchResult
                {
                    Id = reader.GetInt32(0),
                    City = reader.GetString(2),
                    Humidity = reader.GetInt32(3),
                    TempMin = reader.GetDouble(4),
                    TempMax = reader.GetDouble(5),
                    SearchDate = reader.GetDateTime(6),
                    Username = reader.GetString(7),
                    Condition = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CurrentTemp = reader.IsDBNull(9) ? (double?)null : reader.GetDouble(9),
                    WindSpeed = reader.IsDBNull(10) ? (double?)null : reader.GetDouble(10),
                    WindDeg = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                    Icon = reader.IsDBNull(12) ? null : reader.GetString(12)
                });
            }
            return results;
        }

        public async Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            var results = new List<WeatherSearchResult>();
            int totalCount = 0;
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            using var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_GetWeatherSearchesPaged", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object?)city ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Condition", (object?)condition ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Username", (object?)username ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new WeatherSearchResult
                {
                    Id = reader.GetInt32(0),
                    City = reader.GetString(2),
                    Humidity = reader.GetInt32(3),
                    TempMin = reader.GetDouble(4),
                    TempMax = reader.GetDouble(5),
                    SearchDate = reader.GetDateTime(6),
                    Username = reader.GetString(7),
                    Condition = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CurrentTemp = reader.IsDBNull(9) ? (double?)null : reader.GetDouble(9),
                    WindSpeed = reader.IsDBNull(10) ? (double?)null : reader.GetDouble(10),
                    WindDeg = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                    Icon = reader.IsDBNull(12) ? null : reader.GetString(12)
                });
            }
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(0);
            }
            return (results, totalCount);
        }

        public async Task UpdateWeatherSearchAsync(int weatherSearchId, int userId, int humidity, double tempMin, double tempMax, double? currentTemp, string? condition, double? windSpeed, int? windDeg, string changeType, string oldValue, string newValue)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_UpdateWeatherSearch", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@WeatherSearchId", weatherSearchId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Humidity", humidity);
            cmd.Parameters.AddWithValue("@TempMin", tempMin);
            cmd.Parameters.AddWithValue("@TempMax", tempMax);
            cmd.Parameters.AddWithValue("@CurrentTemp", (object?)currentTemp ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Condition", (object?)condition ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WindSpeed", (object?)windSpeed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WindDeg", (object?)windDeg ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChangeType", changeType);
            cmd.Parameters.AddWithValue("@OldValue", oldValue);
            cmd.Parameters.AddWithValue("@NewValue", newValue);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
        }

        public async Task<WeatherSearchResult?> GetWeatherSearchByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT ws.Id, ws.UserId, ws.City, ws.Humidity, ws.TempMin, ws.TempMax, ws.SearchDate, u.Username, ws.Condition, ws.CurrentTemp, ws.WindSpeed, ws.WindDeg, ws.Icon FROM WeatherSearches ws JOIN Users u ON ws.UserId = u.Id WHERE ws.Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new WeatherSearchResult
                {
                    Id = reader.GetInt32(0),
                    City = reader.GetString(2),
                    Humidity = reader.GetInt32(3),
                    TempMin = reader.GetDouble(4),
                    TempMax = reader.GetDouble(5),
                    SearchDate = reader.GetDateTime(6),
                    Username = reader.GetString(7),
                    Condition = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CurrentTemp = reader.IsDBNull(9) ? (double?)null : reader.GetDouble(9),
                    WindSpeed = reader.IsDBNull(10) ? (double?)null : reader.GetDouble(10),
                    WindDeg = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                    Icon = reader.IsDBNull(12) ? null : reader.GetString(12)
                };
            }
            return null;
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

        public async Task<List<string>> GetAllCitiesAsync()
        {
            var cities = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT DISTINCT City FROM WeatherSearches ORDER BY City", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                cities.Add(reader.GetString(0));
            }
            return cities;
        }
    }
} 