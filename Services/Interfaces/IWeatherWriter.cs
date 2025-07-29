using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherWriter
    {
        Task SaveWeatherSearchAsync(int userId, string city, int humidity, double tempMin, 
            double tempMax, DateTime searchDate, string? condition, double? currentTemp, 
            double? windSpeed, int? windDeg);
        
        Task UpdateWeatherSearchAsync(int weatherSearchId, int userId, int humidity, 
            double tempMin, double tempMax, double? currentTemp, string? condition, 
            double? windSpeed, int? windDeg, string changeType, string oldValue, string newValue);
    }
} 