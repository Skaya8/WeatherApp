namespace WeatherApp.Models
{
    public class WeatherSearchResult
    {
        public int Id { get; set; }
        public string? City { get; set; }
        public int Humidity { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public string? Username { get; set; }
        public DateTime SearchDate { get; set; }
        public string? Condition { get; set; }
        public double? CurrentTemp { get; set; }
        public double? WindSpeed { get; set; }
        public int? WindDeg { get; set; }
        public string? Icon { get; set; }
    }
} 