namespace WeatherApp.Models
{
    public class WeatherChange
    {
        public string FieldName { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
    }
} 