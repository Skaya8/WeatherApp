namespace WeatherApp.Services.Interfaces
{
    public interface IWeatherConditionService
    {
        string NormalizeCondition(string? condition);
        string? ExtractMainCondition(string? condition);
        string[] GetMainConditions();
        string[] GetAllConditions();
        bool IsValidCondition(string? condition);
        string GetIconForCondition(string? condition);
        Dictionary<string, string> GetConditionIconMapping();
    }
} 