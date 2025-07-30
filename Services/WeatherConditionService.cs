using WeatherApp.Services.Constants;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class WeatherConditionService : IWeatherConditionService
    {
        public string NormalizeCondition(string? condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return string.Empty;

            var exactMatch = WeatherConditions.GetConditionByDescription(condition);
            if (exactMatch != null)
                return exactMatch.ToString();

            var mainCondition = ExtractMainCondition(condition);
            if (!string.IsNullOrEmpty(mainCondition))
            {
                var conditions = WeatherConditions.GetConditionsByMain(mainCondition);
                if (conditions.Length > 0)
                    return conditions[0]; 
            }

            return condition;
        }

        public string? ExtractMainCondition(string? condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return null;

            var openParenIndex = condition.IndexOf('(');
            if (openParenIndex > 0)
                return condition.Substring(0, openParenIndex).Trim();

            return condition.Trim();
        }

        public string[] GetMainConditions()
        {
            return WeatherConditions.GetMainConditions();
        }

        public string[] GetAllConditions()
        {
            return WeatherConditions.GetAllConditionDescriptions();
        }

        public bool IsValidCondition(string? condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return false;

            return WeatherConditions.GetConditionByDescription(condition) != null;
        }

        public string GetIconForCondition(string? condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return string.Empty;

            var weatherCondition = WeatherConditions.GetConditionByDescription(condition);
            if (weatherCondition?.Icon != null)
            {
                return $"https://openweathermap.org/img/wn/{weatherCondition.Icon}.png";
            }

            var mainCondition = ExtractMainCondition(condition);
            if (!string.IsNullOrEmpty(mainCondition))
            {
                var conditions = WeatherConditions.GetConditionsByMain(mainCondition);
                if (conditions.Length > 0)
                {
                    var descriptionPart = condition.Replace($"{mainCondition} (", "").Replace(")", "");
                    var matchingCondition = WeatherConditions.AllConditions.FirstOrDefault(c => 
                        c.Main.Equals(mainCondition, StringComparison.OrdinalIgnoreCase) && 
                        c.Description.Equals(descriptionPart, StringComparison.OrdinalIgnoreCase));
                    
                    if (matchingCondition?.Icon != null)
                    {
                        return $"https://openweathermap.org/img/wn/{matchingCondition.Icon}.png";
                    }
                }
            }

            return string.Empty;
        }

        public Dictionary<string, string> GetConditionIconMapping()
        {
            var mapping = new Dictionary<string, string>();
            foreach (var condition in WeatherConditions.AllConditions)
            {
                var fullCondition = condition.ToString();
                mapping[fullCondition] = $"https://openweathermap.org/img/wn/{condition.Icon}.png";
            }
            return mapping;
        }
    }
} 