using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WeatherApp.Services.Interfaces;
using WeatherApp.Services.Repositories;

namespace WeatherApp.Services.Repositories
{
    public class ConditionRepository : IConditionRepository
    {
        private readonly string _connectionString;

        public ConditionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<string>> GetAllConditionsAsync()
        {
            var conditions = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(RepositoryConstants.GetAllConditions, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                conditions.Add(reader.GetString(0));
            }
            return conditions;
        }
    }
} 