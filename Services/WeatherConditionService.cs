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

            // Try to find exact match first
            var exactMatch = WeatherConditions.GetConditionByDescription(condition);
            if (exactMatch != null)
                return exactMatch.ToString();

            // Try to match by main condition
            var mainCondition = ExtractMainCondition(condition);
            if (!string.IsNullOrEmpty(mainCondition))
            {
                var conditions = WeatherConditions.GetConditionsByMain(mainCondition);
                if (conditions.Length > 0)
                    return conditions[0]; // Return first match for that main condition
            }

            // If no match found, return the original condition
            return condition;
        }

        public string? ExtractMainCondition(string? condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return null;

            // Extract main condition from "Main (description)" format
            var openParenIndex = condition.IndexOf('(');
            if (openParenIndex > 0)
                return condition.Substring(0, openParenIndex).Trim();

            // If no parentheses, assume the whole string is the main condition
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

            // First try to find exact match
            var weatherCondition = WeatherConditions.GetConditionByDescription(condition);
            if (weatherCondition?.Icon != null)
            {
                return $"https://openweathermap.org/img/wn/{weatherCondition.Icon}.png";
            }

            // If no exact match, try to extract the description from "Main (description)" format
            var mainCondition = ExtractMainCondition(condition);
            if (!string.IsNullOrEmpty(mainCondition))
            {
                var conditions = WeatherConditions.GetConditionsByMain(mainCondition);
                if (conditions.Length > 0)
                {
                    // Find the condition that matches the description part
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