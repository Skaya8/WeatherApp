using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherRepository
    {
        Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null, 
            DateTime? fromDate = null, DateTime? toDate = null);
        
        Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, 
            DateTime? fromDate, DateTime? toDate, int page, int pageSize);
        
        Task SaveWeatherSearchAsync(int userId, string city, int humidity, double tempMin, 
            double tempMax, DateTime searchDate, string? condition, double? currentTemp, 
            double? windSpeed, int? windDeg);
        
        Task UpdateWeatherSearchAsync(int weatherSearchId, int userId, int humidity, 
            double tempMin, double tempMax, double? currentTemp, string? condition, 
            double? windSpeed, int? windDeg, string changeType, string oldValue, string newValue);
        
        Task<WeatherSearchResult?> GetWeatherSearchByIdAsync(int id);
    }

    public interface IUserRepository
    {
        Task<int?> ValidateUserAsync(string username, string password);
    }

    public interface IChangeLogRepository
    {
        Task<List<WeatherSearchChangeLog>> GetWeatherSearchChangesAsync(int weatherSearchId);
    }

    public interface ICityRepository
    {
        Task<List<string>> GetAllCitiesAsync();
    }
} 