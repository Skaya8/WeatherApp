using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly string _connectionString;

        public WeatherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var results = new List<WeatherSearchResult>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetWeatherSearches", conn)
            {
                CommandType = CommandType.StoredProcedure
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
                    WindDeg = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11)
                });
            }
            return results;
        }

        public async Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            var results = new List<WeatherSearchResult>();
            int totalCount = 0;
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetWeatherSearchesPaged", conn)
            {
                CommandType = CommandType.StoredProcedure
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
                    WindDeg = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11)
                });
            }
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(0);
            }
            return (results, totalCount);
        }

        public async Task SaveWeatherSearchAsync(int userId, string city, int humidity, double tempMin, double tempMax, DateTime searchDate,
            string? condition, double? currentTemp, double? windSpeed, int? windDeg)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_InsertWeatherSearch", conn)
            {
                CommandType = CommandType.StoredProcedure
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
            using var cmd = new SqlCommand(RepositoryConstants.GetWeatherSearchById, conn);
            cmd.Parameters.AddWithValue(RepositoryConstants.ParamId, id);

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
                    WindDeg = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11)
                };
            }
            return null;
        }
    }
} 