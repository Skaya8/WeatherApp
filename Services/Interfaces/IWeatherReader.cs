using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherReader
    {
        Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null);
        
        Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, 
            int page, int pageSize);
        
        Task<WeatherSearchResult?> GetWeatherSearchByIdAsync(int id);
    }
} 