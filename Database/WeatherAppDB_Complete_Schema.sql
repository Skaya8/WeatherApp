-- =============================================
-- WeatherApp Database Complete Schema
-- Description: Complete database setup for WeatherApp
-- =============================================

USE [WeatherAppDB]
GO

-- =============================================
-- Table: Users
-- Purpose: User authentication and management
-- =============================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Username] [nvarchar](50) NOT NULL,
    [PasswordHash] [varbinary](256) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
) ON [PRIMARY]
GO

-- =============================================
-- Table: WeatherSearches
-- Purpose: Weather data storage and retrieval
-- =============================================
CREATE TABLE [dbo].[WeatherSearches](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL,
    [City] [nvarchar](100) NOT NULL,
    [Humidity] [int] NOT NULL,
    [TempMin] [float] NOT NULL,
    [TempMax] [float] NOT NULL,
    [SearchDate] [date] NOT NULL,
    [Condition] [nvarchar](200) NULL,
    [CurrentTemp] [float] NULL,
    [WindSpeed] [float] NULL,
    [WindDeg] [int] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- =============================================
-- Table: WeatherSearchChangeLog
-- Purpose: Change logging with user tracking
-- =============================================
CREATE TABLE [dbo].[WeatherSearchChangeLog](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [WeatherSearchId] [int] NOT NULL,
    [UserId] [int] NOT NULL,
    [ChangeDate] [datetime] NOT NULL DEFAULT (getdate()),
    [ChangeType] [nvarchar](100) NOT NULL,
    [OldValue] [nvarchar](255) NULL,
    [NewValue] [nvarchar](255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- =============================================
-- Foreign Key Constraints
-- =============================================

-- WeatherSearches -> Users
ALTER TABLE [dbo].[WeatherSearches] 
ADD CONSTRAINT [FK_WeatherSearches_Users] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id])
GO

-- WeatherSearchChangeLog -> WeatherSearches
ALTER TABLE [dbo].[WeatherSearchChangeLog] 
ADD CONSTRAINT [FK_WeatherSearchChangeLog_WeatherSearches] 
FOREIGN KEY([WeatherSearchId]) REFERENCES [dbo].[WeatherSearches] ([Id])
GO

-- WeatherSearchChangeLog -> Users
ALTER TABLE [dbo].[WeatherSearchChangeLog] 
ADD CONSTRAINT [FK_WeatherSearchChangeLog_Users] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id])
GO

-- =============================================
-- Stored Procedures
-- =============================================

-- =============================================
-- Stored Procedure: sp_ValidateUser
-- Purpose: Secure user authentication with SHA2_256 hashing
-- =============================================
CREATE PROCEDURE [dbo].[sp_ValidateUser]
    @Username NVARCHAR(50),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username 
    FROM Users 
    WHERE Username = @Username 
    AND PasswordHash = HASHBYTES('SHA2_256', @Password)
END
GO

-- =============================================
-- Stored Procedure: sp_InsertWeatherSearch
-- Purpose: Weather data insertion
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertWeatherSearch]
    @UserId INT,
    @City NVARCHAR(100),
    @Humidity INT,
    @TempMin FLOAT,
    @TempMax FLOAT,
    @SearchDate DATE,
    @Condition NVARCHAR(200) = NULL,
    @CurrentTemp FLOAT = NULL,
    @WindSpeed FLOAT = NULL,
    @WindDeg INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO WeatherSearches (UserId, City, Humidity, TempMin, TempMax, SearchDate, Condition, CurrentTemp, WindSpeed, WindDeg)
    VALUES (@UserId, @City, @Humidity, @TempMin, @TempMax, @SearchDate, @Condition, @CurrentTemp, @WindSpeed, @WindDeg)
END
GO

-- =============================================
-- Stored Procedure: sp_GetWeatherSearches
-- Purpose: Weather data retrieval with filtering
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetWeatherSearches]
    @UserId INT = NULL,
    @City NVARCHAR(100) = NULL,
    @Condition NVARCHAR(200) = NULL,
    @Username NVARCHAR(50) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ws.Id, ws.UserId, ws.City, ws.Humidity, ws.TempMin, ws.TempMax, 
           ws.SearchDate, u.Username, ws.Condition, ws.CurrentTemp, 
           ws.WindSpeed, ws.WindDeg
    FROM WeatherSearches ws 
    JOIN Users u ON ws.UserId = u.Id
    WHERE (@UserId IS NULL OR ws.UserId = @UserId)
    AND (@City IS NULL OR ws.City = @City)
    AND (@Condition IS NULL OR ws.Condition = @Condition)
    AND (@Username IS NULL OR u.Username = @Username)
    AND (@FromDate IS NULL OR ws.SearchDate >= @FromDate)
    AND (@ToDate IS NULL OR ws.SearchDate <= @ToDate)
    ORDER BY ws.SearchDate DESC
END
GO

-- =============================================
-- Stored Procedure: sp_GetWeatherSearchesPaged
-- Purpose: Paginated weather data retrieval
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetWeatherSearchesPaged]
    @UserId INT = NULL,
    @City NVARCHAR(100) = NULL,
    @Condition NVARCHAR(200) = NULL,
    @Username NVARCHAR(50) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get total count
    SELECT COUNT(*)
    FROM WeatherSearches ws 
    JOIN Users u ON ws.UserId = u.Id
    WHERE (@UserId IS NULL OR ws.UserId = @UserId)
    AND (@City IS NULL OR ws.City = @City)
    AND (@Condition IS NULL OR ws.Condition = @Condition)
    AND (@Username IS NULL OR u.Username = @Username)
    AND (@FromDate IS NULL OR ws.SearchDate >= @FromDate)
    AND (@ToDate IS NULL OR ws.SearchDate <= @ToDate);
    
    -- Get paginated results
    SELECT ws.Id, ws.UserId, ws.City, ws.Humidity, ws.TempMin, ws.TempMax, 
           ws.SearchDate, u.Username, ws.Condition, ws.CurrentTemp, 
           ws.WindSpeed, ws.WindDeg
    FROM WeatherSearches ws 
    JOIN Users u ON ws.UserId = u.Id
    WHERE (@UserId IS NULL OR ws.UserId = @UserId)
    AND (@City IS NULL OR ws.City = @City)
    AND (@Condition IS NULL OR ws.Condition = @Condition)
    AND (@Username IS NULL OR u.Username = @Username)
    AND (@FromDate IS NULL OR ws.SearchDate >= @FromDate)
    AND (@ToDate IS NULL OR ws.SearchDate <= @ToDate)
    ORDER BY ws.SearchDate DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

-- =============================================
-- Stored Procedure: sp_UpdateWeatherSearch
-- Purpose: Weather data updates with comprehensive change tracking
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateWeatherSearch]
    @WeatherSearchId INT,
    @UserId INT,
    @Humidity INT,
    @TempMin FLOAT,
    @TempMax FLOAT,
    @CurrentTemp FLOAT = NULL,
    @Condition NVARCHAR(200) = NULL,
    @WindSpeed FLOAT = NULL,
    @WindDeg INT = NULL,
    @ChangeType NVARCHAR(100),
    @OldValue NVARCHAR(500),
    @NewValue NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update the weather search
    UPDATE WeatherSearches 
    SET Humidity = @Humidity,
        TempMin = @TempMin,
        TempMax = @TempMax,
        CurrentTemp = @CurrentTemp,
        Condition = @Condition,
        WindSpeed = @WindSpeed,
        WindDeg = @WindDeg
    WHERE Id = @WeatherSearchId;
    
    -- Log the change
    INSERT INTO WeatherSearchChangeLog (WeatherSearchId, UserId, ChangeType, OldValue, NewValue)
    VALUES (@WeatherSearchId, @UserId, @ChangeType, @OldValue, @NewValue);
END
GO

-- =============================================
-- Stored Procedure: sp_GetWeatherSearchChanges
-- Purpose: Change history retrieval
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetWeatherSearchChanges]
    @WeatherSearchId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, WeatherSearchId, UserId, ChangeDate, ChangeType, OldValue, NewValue
    FROM WeatherSearchChangeLog
    WHERE WeatherSearchId = @WeatherSearchId
    ORDER BY ChangeDate DESC
END
GO

-- =============================================
-- Sample Data
-- =============================================

-- Insert sample users
INSERT INTO Users (Username, PasswordHash) VALUES 
('user1', HASHBYTES('SHA2_256', '12345678')),
('user2', HASHBYTES('SHA2_256', '12345678')),
('user3', HASHBYTES('SHA2_256', '12345678')),
('user4', HASHBYTES('SHA2_256', '12345678')),
('user5', HASHBYTES('SHA2_256', '12345678'))
GO

PRINT 'WeatherApp Database schema created successfully!'
PRINT 'Sample users inserted with password: 12345678'
