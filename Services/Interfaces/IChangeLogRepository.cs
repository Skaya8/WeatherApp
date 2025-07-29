using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IChangeLogRepository
    {
        Task<List<WeatherSearchChangeLog>> GetWeatherSearchChangesAsync(int weatherSearchId);
    }
} 