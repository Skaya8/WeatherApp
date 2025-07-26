# Database Setup Guide

This document provides comprehensive instructions for setting up the WeatherApp database using SQL Server Management Studio (SSMS).

## Overview

The WeatherApp database consists of four main tables and seven stored procedures that handle user authentication, weather data storage, and comprehensive change tracking functionality.

## Prerequisites

- **SQL Server LocalDB** (included with Visual Studio 2022)
- **SQL Server Management Studio (SSMS)** - [Download](https://aka.ms/ssms)
- **Visual Studio 2022** (for LocalDB access)

## Database Schema

### Core Tables

1. **Users** - User authentication and management with secure password hashing
2. **WeatherSearches** - Weather data storage and retrieval
3. **WeatherSearchChanges** - Audit trail for data modifications with user tracking
4. **WeatherSearchChangeLog** - Additional change logging with username tracking

### Stored Procedures

1. **sp_ValidateUser** - Secure user authentication with SHA2_256 hashing
2. **sp_InsertWeatherSearch** - Weather data insertion
3. **sp_GetWeatherSearches** - Weather data retrieval with filtering
4. **sp_GetWeatherSearchesPaged** - Paginated weather data retrieval
5. **sp_UpdateWeatherSearch** - Weather data updates with comprehensive change tracking
6. **sp_GetWeatherSearchChanges** - Change history retrieval

## Installation Instructions

### Step 1: Database Connection

1. Launch **SQL Server Management Studio (SSMS)**
2. Connect to LocalDB using the following parameters:
   - **Server name:** `(localdb)\MSSQLLocalDB`
   - **Authentication:** Windows Authentication
   - Click **Connect**

### Step 2: Database Creation

1. In Object Explorer, right-click on **Databases**
2. Select **New Database**
3. Enter **Database name:** `WeatherAppDB`
4. Click **OK**

### Step 3: Table Creation

Execute the following scripts in order:

#### Users Table
```sql
USE WeatherAppDB;
GO

CREATE TABLE [dbo].[Users](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Username] [nvarchar](50) NOT NULL,
    [PasswordHash] [varbinary](256) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);
GO
```

#### WeatherSearches Table
```sql
CREATE TABLE [dbo].[WeatherSearches](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL,
    [City] [nvarchar](100) NOT NULL,
    [Humidity] [int] NOT NULL,
    [TempMin] [float] NOT NULL,
    [TempMax] [float] NOT NULL,
    [SearchDate] [date] NOT NULL,
    [Condition] [nvarchar](100) NULL,
    [CurrentTemp] [float] NULL,
    [WindSpeed] [float] NULL,
    [WindDeg] [int] NULL,
    [Icon] [nvarchar](255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

ALTER TABLE [dbo].[WeatherSearches] 
ADD CONSTRAINT [FK_WeatherSearches_Users] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id]);
GO
```

#### WeatherSearchChanges Table
```sql
CREATE TABLE [dbo].[WeatherSearchChanges](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [WeatherSearchId] [int] NOT NULL,
    [UserId] [int] NOT NULL,
    [ChangeType] [nvarchar](100) NOT NULL,
    [ChangeDate] [datetime] NOT NULL DEFAULT (getdate()),
    [OldValue] [nvarchar](500) NULL,
    [NewValue] [nvarchar](500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

ALTER TABLE [dbo].[WeatherSearchChanges] 
ADD CONSTRAINT [FK_WeatherSearchChanges_Users] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id]);
GO

ALTER TABLE [dbo].[WeatherSearchChanges] 
ADD CONSTRAINT [FK_WeatherSearchChanges_WeatherSearches] 
FOREIGN KEY([WeatherSearchId]) REFERENCES [dbo].[WeatherSearches] ([Id]);
GO
```

#### WeatherSearchChangeLog Table
```sql
CREATE TABLE [dbo].[WeatherSearchChangeLog](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [WeatherSearchId] [int] NOT NULL,
    [ChangeDate] [datetime] NOT NULL DEFAULT (getdate()),
    [ChangeType] [nvarchar](100) NOT NULL,
    [OldValue] [nvarchar](255) NULL,
    [NewValue] [nvarchar](255) NULL,
    [Username] [nvarchar](100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
```

### Step 4: Stored Procedure Creation

Execute each stored procedure script:

#### Secure User Authentication
```sql
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
```

#### Weather Data Insertion
```sql
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
```

#### Weather Data Retrieval
```sql
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
```

#### Paginated Weather Data Retrieval
```sql
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
```

#### Weather Data Updates with Change Tracking
```sql
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

    INSERT INTO WeatherSearchChangeLog (WeatherSearchId, ChangeDate, ChangeType, OldValue, NewValue, Username)
    VALUES (@WeatherSearchId, GETDATE(), @ChangeType, @OldValue, @NewValue, (SELECT Username FROM Users WHERE Id = @UserId));
END
GO
```

#### Change History Retrieval
```sql
CREATE PROCEDURE [dbo].[sp_GetWeatherSearchChanges]
    @WeatherSearchId INT
AS
BEGIN
    SELECT *
    FROM WeatherSearchChangeLog
    WHERE WeatherSearchId = @WeatherSearchId
    ORDER BY ChangeDate DESC
END
GO
```

### Step 5: Test Data Initialization

Initialize the database with test user accounts using secure password hashing:

```sql
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
```

## Configuration

### Connection String

Ensure your `appsettings.json` contains the correct connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=WeatherAppDB;Trusted_Connection=True;"
  }
}
```

### Alternative Server Configurations

For different SQL Server instances, modify the Server parameter:

- **LocalDB:** `(localdb)\MSSQLLocalDB`
- **SQL Server Express:** `.\SQLEXPRESS`
- **SQL Server:** `localhost` or your server name
- **Named Instance:** `SERVERNAME\INSTANCENAME`

## Security Features

### Password Security
- Passwords are stored as SHA2_256 hashes using `HASHBYTES('SHA2_256', @Password)`
- No plain text passwords are stored in the database
- Secure authentication through stored procedure validation

### Change Tracking
- Comprehensive audit trail with `WeatherSearchChanges` and `WeatherSearchChangeLog` tables
- User tracking for all modifications
- Timestamp tracking for all changes

## Verification

After completing the setup, verify the installation:

1. **Check Tables:** Expand `WeatherAppDB` → `Tables` in Object Explorer
2. **Check Stored Procedures:** Expand `WeatherAppDB` → `Programmability` → `Stored Procedures`
3. **Test Authentication:** Execute `EXEC sp_ValidateUser 'user1', '12345678';` to verify secure login
4. **Test Connection:** Execute `SELECT * FROM Users;` to verify data access

## Troubleshooting

### Common Issues

**Authentication Errors**
- Ensure Windows Authentication is enabled
- Verify the connection string uses `Trusted_Connection=True`
- For SQL Server Authentication, use `User ID` and `Password` parameters
- Check that passwords are properly hashed using SHA2_256

**Database Not Found**
- Verify the database name in the connection string matches the created database
- Ensure you're connected to the correct SQL Server instance
- Check that the database creation script executed successfully

**Stored Procedure Errors**
- Execute all stored procedure creation scripts in the correct order
- Verify you're connected to the `WeatherAppDB` database
- Check for syntax errors in the stored procedure scripts
- Ensure all foreign key constraints are properly created

**Permission Issues**
- Ensure your Windows account has appropriate database permissions
- For LocalDB, Windows Authentication should work without additional configuration

## Database Maintenance

### Backup Recommendations
- Create regular backups of the `WeatherAppDB` database
- Store backups in a secure location
- Test backup restoration procedures

### Performance Considerations
- Monitor query performance using SQL Server Profiler
- Consider indexing frequently queried columns
- Regular maintenance of database statistics
- Monitor the change log tables for growth

## Support

For database-related issues or questions, refer to:
- SQL Server Management Studio documentation
- Microsoft SQL Server LocalDB documentation
- The main project README for application-specific guidance 