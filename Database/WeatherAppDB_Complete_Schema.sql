-- =============================================
-- WeatherApp Database Complete Schema
-- Created: 2025-01-26
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
    [Icon] [nvarchar](255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- =============================================
-- Table: WeatherSearchChanges
-- Purpose: Audit trail for data modifications with user tracking
-- =============================================
CREATE TABLE [dbo].[WeatherSearchChanges](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [WeatherSearchId] [int] NOT NULL,
    [UserId] [int] NOT NULL,
    [ChangeType] [nvarchar](100) NOT NULL,
    [ChangeDate] [datetime] NOT NULL DEFAULT (getdate()),
    [OldValue] [nvarchar](500) NULL,
    [NewValue] [nvarchar](500) NULL,
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
    [UserId] [int] NULL,
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

-- WeatherSearchChanges -> Users
ALTER TABLE [dbo].[WeatherSearchChanges] 
ADD CONSTRAINT [FK_WeatherSearchChanges_Users] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id])
GO

-- WeatherSearchChanges -> WeatherSearches
ALTER TABLE [dbo].[WeatherSearchChanges] 
ADD CONSTRAINT [FK_WeatherSearchChanges_WeatherSearches] 
FOREIGN KEY([WeatherSearchId]) REFERENCES [dbo].[WeatherSearches] ([Id])
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
-- Procedure: sp_ValidateUser
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
-- Procedure: sp_InsertWeatherSearch
-- Purpose: Weather data insertion
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertWeatherSearch] 
    @UserId INT, 
    @City NVARCHAR(100), 
    @Humidity INT, 
    @TempMin FLOAT, 
    @TempMax FLOAT, 
    @SearchDate DATE, 
    @Condition NVARCHAR(100) = NULL, 
    @CurrentTemp FLOAT = NULL, 
    @WindSpeed FLOAT = NULL, 
    @WindDeg INT = NULL
AS 
BEGIN 
    INSERT INTO WeatherSearches ( 
        UserId, 
        City, 
        Humidity, 
        TempMin, 
        TempMax, 
        SearchDate, 
        Condition, 
        CurrentTemp, 
        WindSpeed, 
        WindDeg 
    ) 
    VALUES ( 
        @UserId, 
        @City, 
        @Humidity, 
        @TempMin, 
        @TempMax, 
        @SearchDate, 
        @Condition, 
        @CurrentTemp, 
        @WindSpeed, 
        @WindDeg 
    ) 
END
GO

-- =============================================
-- Procedure: sp_GetWeatherSearches
-- Purpose: Weather data retrieval with filtering
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetWeatherSearches]
    @UserId INT = NULL,
    @City NVARCHAR(100) = NULL,
    @Username NVARCHAR(100) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SELECT 
        ws.Id,
        ws.UserId,
        ws.City,
        ws.Humidity,
        ws.TempMin,
        ws.TempMax,
        ws.SearchDate,
        u.Username,
        ws.Condition,
        ws.CurrentTemp,
        ws.WindSpeed,
        ws.WindDeg,
        ws.Icon
    FROM WeatherSearches ws
    JOIN Users u ON ws.UserId = u.Id
    WHERE (@UserId IS NULL OR ws.UserId = @UserId)
      AND (@City IS NULL OR ws.City = @City)
      AND (@Username IS NULL OR u.Username = @Username)
      AND (@FromDate IS NULL OR ws.SearchDate >= @FromDate)
      AND (@ToDate IS NULL OR ws.SearchDate <= @ToDate)
    ORDER BY ws.SearchDate DESC
END
GO

-- =============================================
-- Procedure: sp_GetWeatherSearchesPaged
-- Purpose: Paginated weather data retrieval
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetWeatherSearchesPaged]
    @UserId INT = NULL,
    @City NVARCHAR(100) = NULL,
    @Condition NVARCHAR(100) = NULL,
    @Username NVARCHAR(100) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ws.Id, ws.UserId, ws.City, ws.Humidity, ws.TempMin, ws.TempMax, ws.SearchDate, u.Username, ws.Condition, ws.CurrentTemp, ws.WindSpeed, ws.WindDeg, ws.Icon
    FROM WeatherSearches ws
    JOIN Users u ON ws.UserId = u.Id
    WHERE (@UserId IS NULL OR ws.UserId = @UserId)
      AND (@City IS NULL OR ws.City = @City)
      AND (@Condition IS NULL OR ws.Condition = @Condition)
      AND (@Username IS NULL OR u.Username = @Username)
      AND (@FromDate IS NULL OR ws.SearchDate >= @FromDate)
      AND (@ToDate IS NULL OR ws.SearchDate <= @ToDate)
    ORDER BY ws.SearchDate DESC, ws.Id DESC
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*)
    FROM WeatherSearches ws
    JOIN Users u ON ws.UserId = u.Id
    WHERE (@UserId IS NULL OR ws.UserId = @UserId)
      AND (@City IS NULL OR ws.City = @City)
      AND (@Condition IS NULL OR ws.Condition = @Condition)
      AND (@Username IS NULL OR u.Username = @Username)
      AND (@FromDate IS NULL OR ws.SearchDate >= @FromDate)
      AND (@ToDate IS NULL OR ws.SearchDate <= @ToDate)
END
GO

-- =============================================
-- Procedure: sp_UpdateWeatherSearch
-- Purpose: Weather data updates with comprehensive change tracking
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateWeatherSearch]
    @WeatherSearchId INT,
    @UserId INT,
    @Humidity INT,
    @TempMin FLOAT,
    @TempMax FLOAT,
    @CurrentTemp FLOAT = NULL,
    @Condition NVARCHAR(100) = NULL,
    @WindSpeed FLOAT = NULL,
    @WindDeg INT = NULL,
    @ChangeType NVARCHAR(50),
    @OldValue NVARCHAR(255),
    @NewValue NVARCHAR(255)
AS
BEGIN
    UPDATE WeatherSearches
    SET Humidity = @Humidity,
        TempMin = @TempMin,
        TempMax = @TempMax,
        CurrentTemp = @CurrentTemp,
        Condition = @Condition,
        WindSpeed = @WindSpeed,
        WindDeg = @WindDeg
    WHERE Id = @WeatherSearchId;

    INSERT INTO WeatherSearchChangeLog (WeatherSearchId, UserId, ChangeDate, ChangeType, OldValue, NewValue)
    VALUES (@WeatherSearchId, @UserId, GETDATE(), @ChangeType, @OldValue, @NewValue);
END
GO

-- =============================================
-- Procedure: sp_GetWeatherSearchChanges
-- Purpose: Change history retrieval
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetWeatherSearchChanges]
    @WeatherSearchId INT
AS
BEGIN
    SELECT Id, WeatherSearchId, UserId, ChangeDate, ChangeType, OldValue, NewValue
    FROM WeatherSearchChangeLog
    WHERE WeatherSearchId = @WeatherSearchId
    ORDER BY ChangeDate DESC
END
GO

-- =============================================
-- Test Data Initialization
-- =============================================

-- Insert test users with SHA2_256 hashed passwords
INSERT INTO Users (Username, PasswordHash) 
VALUES ('user1', HASHBYTES('SHA2_256', '12345678'));
INSERT INTO Users (Username, PasswordHash) 
VALUES ('user2', HASHBYTES('SHA2_256', '12345678'));
INSERT INTO Users (Username, PasswordHash) 
VALUES ('user3', HASHBYTES('SHA2_256', '12345678'));
INSERT INTO Users (Username, PasswordHash) 
VALUES ('user4', HASHBYTES('SHA2_256', '12345678'));
INSERT INTO Users (Username, PasswordHash) 
VALUES ('user5', HASHBYTES('SHA2_256', '12345678'));
GO

-- =============================================
-- Verification Queries
-- =============================================

-- Verify tables were created
SELECT 'Tables Created:' as Status;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = 'dbo';
GO

-- Verify stored procedures were created
SELECT 'Stored Procedures Created:' as Status;
SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' AND ROUTINE_SCHEMA = 'dbo';
GO

-- Verify test users were inserted
SELECT 'Test Users:' as Status;
SELECT Id, Username FROM Users;
GO

-- Test authentication
SELECT 'Authentication Test:' as Status;
EXEC sp_ValidateUser 'user1', '12345678';
GO

-- =============================================
-- End of WeatherApp Database Schema
-- ============================================= 