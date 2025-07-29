namespace WeatherApp.Services.Interfaces
{
    public interface IConditionRepository
    {
        Task<List<string>> GetAllConditionsAsync();
    }
} 