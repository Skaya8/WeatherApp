namespace WeatherApp.Models
{
    public class WeatherSearchChangeLog
    {
        public DateTime ChangeDate { get; set; }
        public string? ChangeType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int? UserId { get; set; }
    }
} 