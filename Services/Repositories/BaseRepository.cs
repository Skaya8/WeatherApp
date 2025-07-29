using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WeatherApp.Services.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;
        protected readonly ILogger _logger;

        protected BaseRepository(IConfiguration configuration, ILogger logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<SqlConnection> CreateConnectionAsync()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Connection string is null or empty.");

            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        protected static SqlParameter CreateParameter(string name, object? value)
        {
            ValidateParameterName(name);
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        private static void ValidateParameterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Parameter name cannot be null or empty.", nameof(name));
        }

        private static void ValidateCommandText(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentException("Command text cannot be null or empty.", nameof(commandText));
        }

        protected async Task<T?> ExecuteScalarAsync<T>(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            ValidateCommandText(commandText);

            try
            {
                using var connection = await CreateConnectionAsync();
                using var command = CreateCommand(commandText, connection, commandType, parameters);

                var result = await command.ExecuteScalarAsync();
                return result != DBNull.Value ? (T?)result : default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scalar query: {CommandText}", commandText);
                throw;
            }
        }

        protected async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            ValidateCommandText(commandText);

            try
            {
                using var connection = await CreateConnectionAsync();
                using var command = CreateCommand(commandText, connection, commandType, parameters);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing non-query: {CommandText}", commandText);
                throw;
            }
        }

        protected async Task<SqlDataReader> ExecuteReaderAsync(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            ValidateCommandText(commandText);

            try
            {
                var connection = await CreateConnectionAsync();
                var command = CreateCommand(commandText, connection, commandType, parameters);

                return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing reader query: {CommandText}", commandText);
                throw;
            }
        }

        private static SqlCommand CreateCommand(string commandText, SqlConnection connection, CommandType commandType, SqlParameter[] parameters)
        {
            var command = new SqlCommand(commandText, connection)
            {
                CommandType = commandType
            };
            
            if (parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            return command;
        }
    }
} 