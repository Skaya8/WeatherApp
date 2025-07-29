using WeatherApp.Models;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IChangeLogRepository _changeLogRepository;
        private readonly IWeatherConditionService _weatherConditionService;
        private readonly IChangeDetectionService _changeDetectionService;
        private readonly IValidationService _validationService;

        public WeatherService(
            IWeatherRepository weatherRepository, 
            IChangeLogRepository changeLogRepository,
            IWeatherConditionService weatherConditionService,
            IChangeDetectionService changeDetectionService,
            IValidationService validationService)
        {
            _weatherRepository = weatherRepository;
            _changeLogRepository = changeLogRepository;
            _weatherConditionService = weatherConditionService;
            _changeDetectionService = changeDetectionService;
            _validationService = validationService;
        }

        public async Task<List<WeatherSearchResult>> GetWeatherSearchesAsync(
            int? userId = null, string? city = null, string? username = null)
        {
            return await _weatherRepository.GetWeatherSearchesAsync(userId, city, username);
        }

        public async Task<(List<WeatherSearchResult> Results, int TotalCount)> GetWeatherSearchesPagedAsync(
            int? userId, string? city, string? condition, string? username, 
            int page, int pageSize)
        {
            return await _weatherRepository.GetWeatherSearchesPagedAsync(
                userId, city, condition, username, page, pageSize);
        }

        public async Task SaveWeatherSearchAsync(WeatherViewModel model, int userId)
        {
            if (!_validationService.IsValidWeatherData(model))
                throw new ArgumentException("Invalid weather data provided");

            if (!_validationService.IsValidUserId(userId))
                throw new ArgumentException("Invalid user ID provided");

            // Normalize the condition to match our comprehensive list
            var normalizedCondition = _weatherConditionService.NormalizeCondition(model.Condition);
            
            await _weatherRepository.SaveWeatherSearchAsync(
                userId,
                model.City ?? "",
                model.Humidity ?? 0,
                model.TempMin ?? 0,
                model.TempMax ?? 0,
                DateTime.Today,
                normalizedCondition,
                model.CurrentTemp,
                model.WindSpeed,
                model.WindDeg
            );
        }

        public async Task UpdateWeatherSearchAsync(WeatherSearchResult change, int userId)
        {
            var old = await _weatherRepository.GetWeatherSearchByIdAsync(change.Id);
            if (old == null) return;

            var changes = await _changeDetectionService.DetectChangesAsync(old, change);
            
            foreach (var fieldChange in changes)
            {
                await _weatherRepository.UpdateWeatherSearchAsync(
                    change.Id, userId, change.Humidity, change.TempMin, change.TempMax,
                    change.CurrentTemp, change.Condition, change.WindSpeed, change.WindDeg,
                    fieldChange.FieldName, fieldChange.OldValue, fieldChange.NewValue);
            }
        }

        public async Task<List<WeatherSearchChangeLog>> GetChangeLogAsync(int weatherSearchId)
        {
            return await _changeLogRepository.GetWeatherSearchChangesAsync(weatherSearchId);
        }


    }
} 