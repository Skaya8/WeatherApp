using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class ValidationService : IValidationService
    {
        private const double MinTemperature = -100;
        private const double MaxTemperature = 100;
        private const int MinHumidity = 0;
        private const int MaxHumidity = 100;

        public bool IsValidWeatherData(WeatherViewModel model)
        {
            if (model == null) return false;

            return IsValidCity(model.City) &&
                   IsValidTemperature(model.TempMin) &&
                   IsValidTemperature(model.TempMax) &&
                   IsValidHumidity(model.Humidity);
        }

        public bool IsValidUserId(int userId)
        {
            return userId > 0;
        }

        public bool IsValidCity(string? city)
        {
            return !string.IsNullOrWhiteSpace(city) && city.Length <= 100;
        }

        public bool IsValidTemperature(double? temperature)
        {
            return temperature.HasValue && 
                   temperature.Value >= MinTemperature && 
                   temperature.Value <= MaxTemperature;
        }

        public bool IsValidHumidity(int? humidity)
        {
            return humidity.HasValue && 
                   humidity.Value >= MinHumidity && 
                   humidity.Value <= MaxHumidity;
        }

        public ValidationResult ValidateWeatherSearch(WeatherSearchResult result)
        {
            var validationResult = new ValidationResult { IsValid = true };

            if (!IsValidCity(result.City))
            {
                validationResult.IsValid = false;
                validationResult.Errors.Add("Invalid city name");
            }

            if (!IsValidTemperature(result.TempMin))
            {
                validationResult.IsValid = false;
                validationResult.Errors.Add("Invalid minimum temperature");
            }

            if (!IsValidTemperature(result.TempMax))
            {
                validationResult.IsValid = false;
                validationResult.Errors.Add("Invalid maximum temperature");
            }

            if (!IsValidHumidity(result.Humidity))
            {
                validationResult.IsValid = false;
                validationResult.Errors.Add("Invalid humidity value");
            }

            return validationResult;
        }
    }
} 