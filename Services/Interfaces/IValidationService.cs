using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IValidationService
    {
        bool IsValidWeatherData(WeatherViewModel model);
        bool IsValidUserId(int userId);
        bool IsValidCity(string? city);
        bool IsValidTemperature(double? temperature);
        bool IsValidHumidity(int? humidity);
        ValidationResult ValidateWeatherSearch(WeatherSearchResult result);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
} 