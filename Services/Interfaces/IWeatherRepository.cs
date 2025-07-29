using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherRepository : IWeatherReader, IWeatherWriter
    {
    }
} 