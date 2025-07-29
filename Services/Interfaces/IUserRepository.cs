using WeatherApp.Models;

namespace WeatherApp.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<int?> ValidateUserAsync(string username, string password);
    }
} 