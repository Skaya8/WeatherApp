using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherReader
    {
        Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null, 
            DateTime? fromDate = null, DateTime? toDate = null);
        
        Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, 
            DateTime? fromDate, DateTime? toDate, int page, int pageSize);
        
        Task<WeatherSearchResult?> GetWeatherSearchByIdAsync(int id);
    }
} 