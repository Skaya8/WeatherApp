using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IChangeDetectionService
    {
        Task<List<WeatherChange>> DetectChangesAsync(WeatherSearchResult old, WeatherSearchResult change);
        bool HasCurrentTempChanged(WeatherSearchResult old, WeatherSearchResult change);
        bool HasWindSpeedChanged(WeatherSearchResult old, WeatherSearchResult change);
        bool HasTemperatureChanged(double oldTemp, double newTemp);
    }
} 