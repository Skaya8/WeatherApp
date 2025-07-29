namespace WeatherApp.Services.Repositories
{
    public static class RepositoryConstants
    {
        // Stored Procedures
        public const string SpValidateUser = "sp_ValidateUser";
        public const string SpGetWeatherSearches = "sp_GetWeatherSearches";
        public const string SpGetWeatherSearchesPaged = "sp_GetWeatherSearchesPaged";
        public const string SpInsertWeatherSearch = "sp_InsertWeatherSearch";
        public const string SpUpdateWeatherSearch = "sp_UpdateWeatherSearch";
        public const string SpGetWeatherSearchChanges = "sp_GetWeatherSearchChanges";

        // SQL Queries
        public const string GetWeatherSearchById = @"
            SELECT ws.Id, ws.UserId, ws.City, ws.Humidity, ws.TempMin, ws.TempMax, 
                   ws.SearchDate, u.Username, ws.Condition, ws.CurrentTemp, 
                   ws.WindSpeed, ws.WindDeg
            FROM WeatherSearches ws 
            JOIN Users u ON ws.UserId = u.Id 
            WHERE ws.Id = @Id";

        public const string GetAllCities = "SELECT DISTINCT City FROM WeatherSearches ORDER BY City";
        public const string GetAllConditions = "SELECT DISTINCT Condition FROM WeatherSearches WHERE Condition IS NOT NULL AND Condition != '' ORDER BY Condition";

        public const string ValidateUserQuery = @"
            SELECT Id, Username 
            FROM Users 
            WHERE Username = @Username 
            AND PasswordHash = HASHBYTES('SHA2_256', @Password)";

        // Parameter Names
        public const string ParamUserId = "@UserId";
        public const string ParamCity = "@City";
        public const string ParamUsername = "@Username";
        public const string ParamFromDate = "@FromDate";
        public const string ParamToDate = "@ToDate";
        public const string ParamCondition = "@Condition";
        public const string ParamPage = "@Page";
        public const string ParamPageSize = "@PageSize";
        public const string ParamHumidity = "@Humidity";
        public const string ParamTempMin = "@TempMin";
        public const string ParamTempMax = "@TempMax";
        public const string ParamSearchDate = "@SearchDate";
        public const string ParamCurrentTemp = "@CurrentTemp";
        public const string ParamWindSpeed = "@WindSpeed";
        public const string ParamWindDeg = "@WindDeg";
        public const string ParamWeatherSearchId = "@WeatherSearchId";
        public const string ParamChangeType = "@ChangeType";
        public const string ParamOldValue = "@OldValue";
        public const string ParamNewValue = "@NewValue";
        public const string ParamId = "@Id";
        public const string ParamPassword = "@Password";
    }
} 