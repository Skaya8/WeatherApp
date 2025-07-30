using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IValidationService
    {
        /// <summary>
        /// Validates user credentials and returns the user ID if valid
        /// </summary>
        /// <param name="username">The username to validate</param>
        /// <param name="password">The password to validate</param>
        /// <returns>User ID if valid, null otherwise</returns>
        Task<int?> ValidateUserCredentialsAsync(string username, string password);

        /// <summary>
        /// Validates if a username is available for registration
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if username is available, false otherwise</returns>
        Task<bool> IsUsernameAvailableAsync(string username);

        /// <summary>
        /// Validates email format
        /// </summary>
        /// <param name="email">The email to validate</param>
        /// <returns>True if email format is valid, false otherwise</returns>
        bool IsValidEmailFormat(string email);

        /// <summary>
        /// Validates password strength
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <returns>Validation result with details about password strength</returns>
        PasswordValidationResult ValidatePasswordStrength(string password);

        /// <summary>
        /// Validates weather data before saving
        /// </summary>
        /// <param name="weatherData">The weather data to validate</param>
        /// <returns>Validation result with any validation errors</returns>
        ValidationResult ValidateWeatherData(object weatherData);
    }

    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int StrengthScore { get; set; } // 0-100
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<string, string> FieldErrors { get; set; } = new Dictionary<string, string>();
    }
} 