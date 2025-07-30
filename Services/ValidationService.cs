using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class ValidationService : IValidationService
    {
        private readonly string _connectionString;

        public ValidationService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<int?> ValidateUserCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine($"Validation failed: Username or password is null/empty");
                return null;
            }

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_ValidateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var userId = reader.GetInt32(0);
                    Console.WriteLine($"User validation successful for username: {username}, UserId: {userId}");
                    return userId;
                }
                
                Console.WriteLine($"User validation failed for username: {username} - no matching user found");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during user validation for username {username}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", conn);
            
            cmd.Parameters.AddWithValue("@Username", username);

            await conn.OpenAsync();
            var count = await cmd.ExecuteScalarAsync();
            
            return Convert.ToInt32(count) == 0;
        }

        public bool IsValidEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public PasswordValidationResult ValidatePasswordStrength(string password)
        {
            var result = new PasswordValidationResult();
            var score = 0;

            if (string.IsNullOrWhiteSpace(password))
            {
                result.Errors.Add("Password cannot be empty");
                return result;
            }

            // Length check
            if (password.Length >= 8)
                score += 20;
            else
                result.Errors.Add("Password must be at least 8 characters long");

            if (password.Length >= 12)
                score += 10;

            // Character variety checks
            if (Regex.IsMatch(password, @"[a-z]"))
                score += 10;
            else
                result.Errors.Add("Password must contain at least one lowercase letter");

            if (Regex.IsMatch(password, @"[A-Z]"))
                score += 10;
            else
                result.Errors.Add("Password must contain at least one uppercase letter");

            if (Regex.IsMatch(password, @"\d"))
                score += 10;
            else
                result.Errors.Add("Password must contain at least one number");

            if (Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
                score += 10;
            else
                result.Errors.Add("Password must contain at least one special character");

            // Complexity bonus
            if (password.Length >= 8 && 
                Regex.IsMatch(password, @"[a-z]") && 
                Regex.IsMatch(password, @"[A-Z]") && 
                Regex.IsMatch(password, @"\d") && 
                Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
            {
                score += 20;
            }

            result.StrengthScore = Math.Min(score, 100);
            result.IsValid = result.Errors.Count == 0 && result.StrengthScore >= 60;

            return result;
        }

        public ValidationResult ValidateWeatherData(object weatherData)
        {
            var result = new ValidationResult();
            
            if (weatherData == null)
            {
                result.IsValid = false;
                result.Errors.Add("Weather data cannot be null");
                return result;
            }

            // Use reflection to validate weather data properties
            var properties = weatherData.GetType().GetProperties();
            
            foreach (var property in properties)
            {
                var value = property.GetValue(weatherData);
                
                // Check for required properties
                if ((property.Name == "Temperature" || property.Name == "CurrentTemp") && value != null)
                {
                    if (double.TryParse(value.ToString(), out double temp))
                    {
                        if (temp < -100 || temp > 150)
                        {
                            result.FieldErrors[property.Name] = "Temperature must be between -100 and 150 degrees";
                            result.IsValid = false;
                        }
                    }
                    else
                    {
                        result.FieldErrors[property.Name] = "Temperature must be a valid number";
                        result.IsValid = false;
                    }
                }
                
                if (property.Name == "Humidity" && value != null)
                {
                    if (double.TryParse(value.ToString(), out double humidity))
                    {
                        if (humidity < 0 || humidity > 100)
                        {
                            result.FieldErrors[property.Name] = "Humidity must be between 0 and 100 percent";
                            result.IsValid = false;
                        }
                    }
                    else
                    {
                        result.FieldErrors[property.Name] = "Humidity must be a valid number";
                        result.IsValid = false;
                    }
                }

                // Validate wind speed
                if (property.Name == "WindSpeed" && value != null)
                {
                    if (double.TryParse(value.ToString(), out double windSpeed))
                    {
                        if (windSpeed < 0 || windSpeed > 200)
                        {
                            result.FieldErrors[property.Name] = "Wind speed must be between 0 and 200 km/h";
                            result.IsValid = false;
                        }
                    }
                    else
                    {
                        result.FieldErrors[property.Name] = "Wind speed must be a valid number";
                        result.IsValid = false;
                    }
                }

                // Validate wind direction
                if (property.Name == "WindDeg" && value != null)
                {
                    if (int.TryParse(value.ToString(), out int windDeg))
                    {
                        if (windDeg < 0 || windDeg > 360)
                        {
                            result.FieldErrors[property.Name] = "Wind direction must be between 0 and 360 degrees";
                            result.IsValid = false;
                        }
                    }
                    else
                    {
                        result.FieldErrors[property.Name] = "Wind direction must be a valid number";
                        result.IsValid = false;
                    }
                }

                // Validate city name
                if (property.Name == "City" && value != null)
                {
                    var city = value.ToString();
                    if (string.IsNullOrWhiteSpace(city) || city.Length > 100)
                    {
                        result.FieldErrors[property.Name] = "City name must not be empty and must be less than 100 characters";
                        result.IsValid = false;
                    }
                }
            }

            if (result.FieldErrors.Count == 0)
            {
                result.IsValid = true;
            }

            return result;
        }
    }
} 