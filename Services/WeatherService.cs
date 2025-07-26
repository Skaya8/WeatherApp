using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IChangeLogRepository _changeLogRepository;

        public WeatherService(IWeatherRepository weatherRepository, IChangeLogRepository changeLogRepository)
        {
            _weatherRepository = weatherRepository;
            _changeLogRepository = changeLogRepository;
        }

        public async Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null, 
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await _weatherRepository.GetWeatherSearchesAsync(userId, city, username, fromDate, toDate);
        }

        public async Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, 
            DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            return await _weatherRepository.GetWeatherSearchesPagedAsync(
                userId, city, condition, username, fromDate, toDate, page, pageSize);
        }

        public async Task SaveWeatherSearchAsync(WeatherViewModel model, int userId)
        {
            await _weatherRepository.SaveWeatherSearchAsync(
                userId,
                model.City ?? "",
                model.Humidity ?? 0,
                model.TempMin ?? 0,
                model.TempMax ?? 0,
                DateTime.Today,
                model.Condition,
                model.CurrentTemp,
                model.WindSpeed,
                model.WindDeg
            );
        }

        public async Task UpdateWeatherSearchAsync(WeatherSearchResult change, int userId)
        {
            var old = await _weatherRepository.GetWeatherSearchByIdAsync(change.Id);
            if (old == null) return;

            await UpdateChangedFields(old, change, userId);
        }

        public async Task<List<WeatherSearchChangeLog>> GetChangeLogAsync(int weatherSearchId)
        {
            return await _changeLogRepository.GetWeatherSearchChangesAsync(weatherSearchId);
        }

        private async Task UpdateChangedFields(WeatherSearchResult old, WeatherSearchResult change, int userId)
        {
            if (old.Humidity != change.Humidity)
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    "Humidity", old.Humidity.ToString(), change.Humidity.ToString());
            }

            if (Math.Abs(old.TempMin - change.TempMin) > 0.0001)
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    "TempMin", old.TempMin.ToString(), change.TempMin.ToString());
            }

            if (Math.Abs(old.TempMax - change.TempMax) > 0.0001)
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    "TempMax", old.TempMax.ToString(), change.TempMax.ToString());
            }

            if (HasCurrentTempChanged(old, change))
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    "CurrentTemp", 
                    old.CurrentTemp.HasValue ? old.CurrentTemp.Value.ToString() : "",
                    change.CurrentTemp.HasValue ? change.CurrentTemp.Value.ToString() : "");
            }

            if (old.Condition != change.Condition)
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    "Condition", old.Condition ?? "", change.Condition ?? "");
            }

            if (HasWindSpeedChanged(old, change))
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    "WindSpeed",
                    old.WindSpeed.HasValue ? old.WindSpeed.Value.ToString() : "",
                    change.WindSpeed.HasValue ? change.WindSpeed.Value.ToString() : "");
            }

            if (old.WindDeg != change.WindDeg)
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    "WindDeg",
                    old.WindDeg.HasValue ? old.WindDeg.Value.ToString() : "",
                    change.WindDeg.HasValue ? change.WindDeg.Value.ToString() : "");
            }
        }

        private static bool HasCurrentTempChanged(WeatherSearchResult old, WeatherSearchResult change)
        {
            return old.CurrentTemp.HasValue && change.CurrentTemp.HasValue && 
                   Math.Abs(old.CurrentTemp.Value - change.CurrentTemp.Value) > 0.0001 ||
                   (old.CurrentTemp.HasValue != change.CurrentTemp.HasValue);
        }

        private static bool HasWindSpeedChanged(WeatherSearchResult old, WeatherSearchResult change)
        {
            return old.WindSpeed.HasValue && change.WindSpeed.HasValue && 
                   Math.Abs(old.WindSpeed.Value - change.WindSpeed.Value) > 0.0001 ||
                   (old.WindSpeed.HasValue != change.WindSpeed.HasValue);
        }
    }
} 