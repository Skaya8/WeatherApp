using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class ChangeDetectionService : IChangeDetectionService
    {
        private const double Tolerance = 0.0001;

        public async Task<List<WeatherChange>> DetectChangesAsync(WeatherSearchResult old, WeatherSearchResult change)
        {
            var changes = new List<WeatherChange>();

            if (old.Humidity != change.Humidity)
            {
                changes.Add(new WeatherChange
                {
                    FieldName = "Humidity",
                    OldValue = old.Humidity.ToString(),
                    NewValue = change.Humidity.ToString()
                });
            }

            if (HasTemperatureChanged(old.TempMin, change.TempMin))
            {
                changes.Add(new WeatherChange
                {
                    FieldName = "TempMin",
                    OldValue = old.TempMin.ToString(),
                    NewValue = change.TempMin.ToString()
                });
            }

            if (HasTemperatureChanged(old.TempMax, change.TempMax))
            {
                changes.Add(new WeatherChange
                {
                    FieldName = "TempMax",
                    OldValue = old.TempMax.ToString(),
                    NewValue = change.TempMax.ToString()
                });
            }

            if (HasCurrentTempChanged(old, change))
            {
                changes.Add(new WeatherChange
                {
                    FieldName = "CurrentTemp",
                    OldValue = old.CurrentTemp?.ToString() ?? "",
                    NewValue = change.CurrentTemp?.ToString() ?? ""
                });
            }

            if (old.Condition != change.Condition)
            {
                changes.Add(new WeatherChange
                {
                    FieldName = "Condition",
                    OldValue = old.Condition ?? "",
                    NewValue = change.Condition ?? ""
                });
            }

            if (HasWindSpeedChanged(old, change))
            {
                changes.Add(new WeatherChange
                {
                    FieldName = "WindSpeed",
                    OldValue = old.WindSpeed?.ToString() ?? "",
                    NewValue = change.WindSpeed?.ToString() ?? ""
                });
            }

            if (old.WindDeg != change.WindDeg)
            {
                changes.Add(new WeatherChange
                {
                    FieldName = "WindDeg",
                    OldValue = old.WindDeg?.ToString() ?? "",
                    NewValue = change.WindDeg?.ToString() ?? ""
                });
            }

            return await Task.FromResult(changes);
        }

        public bool HasCurrentTempChanged(WeatherSearchResult old, WeatherSearchResult change)
        {
            return old.CurrentTemp.HasValue && change.CurrentTemp.HasValue && 
                   HasTemperatureChanged(old.CurrentTemp.Value, change.CurrentTemp.Value) ||
                   (old.CurrentTemp.HasValue != change.CurrentTemp.HasValue);
        }

        public bool HasWindSpeedChanged(WeatherSearchResult old, WeatherSearchResult change)
        {
            return old.WindSpeed.HasValue && change.WindSpeed.HasValue && 
                   HasTemperatureChanged(old.WindSpeed.Value, change.WindSpeed.Value) ||
                   (old.WindSpeed.HasValue != change.WindSpeed.HasValue);
        }

        public bool HasTemperatureChanged(double oldTemp, double newTemp)
        {
            return Math.Abs(oldTemp - newTemp) > Tolerance;
        }
    }
} 