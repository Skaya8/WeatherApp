namespace WeatherApp.Services.Constants
{
    public static class WeatherConditions
    {
        public static readonly WeatherCondition[] AllConditions = new[]
        {
            new WeatherCondition("Thunderstorm", "thunderstorm with light rain", "11d"),
            new WeatherCondition("Thunderstorm", "thunderstorm with rain", "11d"),
            new WeatherCondition("Thunderstorm", "thunderstorm with heavy rain", "11d"),
            new WeatherCondition("Thunderstorm", "light thunderstorm", "11d"),
            new WeatherCondition("Thunderstorm", "thunderstorm", "11d"),
            new WeatherCondition("Thunderstorm", "heavy thunderstorm", "11d"),
            new WeatherCondition("Thunderstorm", "ragged thunderstorm", "11d"),
            new WeatherCondition("Thunderstorm", "thunderstorm with light drizzle", "11d"),
            new WeatherCondition("Thunderstorm", "thunderstorm with drizzle", "11d"),
            new WeatherCondition("Thunderstorm", "thunderstorm with heavy drizzle", "11d"),
            new WeatherCondition("Drizzle", "light intensity drizzle", "09d"),
            new WeatherCondition("Drizzle", "drizzle", "09d"),
            new WeatherCondition("Drizzle", "heavy intensity drizzle", "09d"),
            new WeatherCondition("Drizzle", "light intensity drizzle rain", "09d"),
            new WeatherCondition("Drizzle", "drizzle rain", "09d"),
            new WeatherCondition("Drizzle", "heavy intensity drizzle rain", "09d"),
            new WeatherCondition("Drizzle", "shower rain and drizzle", "09d"),
            new WeatherCondition("Drizzle", "heavy shower rain and drizzle", "09d"),
            new WeatherCondition("Drizzle", "shower drizzle", "09d"),
            new WeatherCondition("Rain", "light rain", "10d"),
            new WeatherCondition("Rain", "moderate rain", "10d"),
            new WeatherCondition("Rain", "heavy intensity rain", "10d"),
            new WeatherCondition("Rain", "very heavy rain", "10d"),
            new WeatherCondition("Rain", "extreme rain", "10d"),
            new WeatherCondition("Rain", "freezing rain", "13d"),
            new WeatherCondition("Rain", "light intensity shower rain", "09d"),
            new WeatherCondition("Rain", "shower rain", "09d"),
            new WeatherCondition("Rain", "heavy intensity shower rain", "09d"),
            new WeatherCondition("Rain", "ragged shower rain", "09d"),
            new WeatherCondition("Snow", "light snow", "13d"),
            new WeatherCondition("Snow", "snow", "13d"),
            new WeatherCondition("Snow", "heavy snow", "13d"),
            new WeatherCondition("Snow", "sleet", "13d"),
            new WeatherCondition("Snow", "light shower sleet", "13d"),
            new WeatherCondition("Snow", "shower sleet", "13d"),
            new WeatherCondition("Snow", "light rain and snow", "13d"),
            new WeatherCondition("Snow", "rain and snow", "13d"),
            new WeatherCondition("Snow", "light shower snow", "13d"),
            new WeatherCondition("Snow", "shower snow", "13d"),
            new WeatherCondition("Snow", "heavy shower snow", "13d"),
            new WeatherCondition("Mist", "mist", "50d"),
            new WeatherCondition("Smoke", "smoke", "50d"),
            new WeatherCondition("Haze", "haze", "50d"),
            new WeatherCondition("Dust", "sand/dust whirls", "50d"),
            new WeatherCondition("Fog", "fog", "50d"),
            new WeatherCondition("Sand", "sand", "50d"),
            new WeatherCondition("Dust", "dust", "50d"),
            new WeatherCondition("Ash", "volcanic ash", "50d"),
            new WeatherCondition("Squall", "squalls", "50d"),
            new WeatherCondition("Tornado", "tornado", "50d"),
            new WeatherCondition("Clear", "clear sky", "01d"),
            new WeatherCondition("Clouds", "few clouds: 11-25%", "02d"),
            new WeatherCondition("Clouds", "scattered clouds: 25-50%", "03d"),
            new WeatherCondition("Clouds", "broken clouds: 51-84%", "04d"),
            new WeatherCondition("Clouds", "overcast clouds: 85-100%", "04d")
        };

        public static string[] GetMainConditions()
        {
            return AllConditions
                .Select(c => c.Main)
                .Distinct()
                .OrderBy(c => c)
                .ToArray();
        }

        public static string[] GetAllConditionDescriptions()
        {
            return AllConditions
                .Select(c => $"{c.Main} ({c.Description})")
                .OrderBy(c => c)
                .ToArray();
        }

        public static WeatherCondition? GetConditionByDescription(string description)
        {
            return AllConditions.FirstOrDefault(c => 
                c.Description.Equals(description, StringComparison.OrdinalIgnoreCase));
        }

        public static string[] GetConditionsByMain(string main)
        {
            return AllConditions
                .Where(c => c.Main.Equals(main, StringComparison.OrdinalIgnoreCase))
                .Select(c => $"{c.Main} ({c.Description})")
                .ToArray();
        }
    }

    public class WeatherCondition
    {
        public string Main { get; }
        public string Description { get; }
        public string Icon { get; }

        public WeatherCondition(string main, string description, string icon)
        {
            Main = main;
            Description = description;
            Icon = icon;
        }

        public override string ToString()
        {
            return $"{Main} ({Description})";
        }
    }
} 