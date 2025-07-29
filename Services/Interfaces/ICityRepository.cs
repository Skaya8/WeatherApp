using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface ICityRepository
    {
        Task<List<string>> GetAllCitiesAsync();
    }
} 