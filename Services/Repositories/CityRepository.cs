using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly string _connectionString;

        public CityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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