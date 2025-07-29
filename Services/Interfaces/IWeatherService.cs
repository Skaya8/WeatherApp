using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null);
        
        Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, 
            int page, int pageSize);
        
        Task SaveWeatherSearchAsync(WeatherViewModel model, int userId);
        Task UpdateWeatherSearchAsync(WeatherSearchResult change, int userId);
        Task<List<WeatherSearchChangeLog>> GetChangeLogAsync(int weatherSearchId);
    }
} 