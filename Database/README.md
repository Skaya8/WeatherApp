# Database Setup Guide

This document provides comprehensive instructions for setting up the WeatherApp database using SQL Server Management Studio (SSMS).

## Overview

The WeatherApp database consists of three main tables and seven stored procedures that handle user authentication, weather data storage, and comprehensive change tracking functionality.

## Prerequisites

- **SQL Server LocalDB** (included with Visual Studio 2022)
- **SQL Server Management Studio (SSMS)** - [Download](https://aka.ms/ssms)
- **Visual Studio 2022** (for LocalDB access)

## Database Schema

### Core Tables

1. **Users** - User authentication and management with secure password hashing
2. **WeatherSearches** - Weather data storage and retrieval (icons generated dynamically)
3. **WeatherSearchChangeLog** - Change logging with user tracking

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

### Step 3: Schema Execution

Execute the complete schema script:
```sql
-- Run the complete schema file
-- File: WeatherAppDB_Complete_Schema.sql
```

## Table Structures

### Users Table
```sql
CREATE TABLE [dbo].[Users](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Username] [nvarchar](50) NOT NULL,
    [PasswordHash] [varbinary](256) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);
```

### WeatherSearches Table
```sql
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
);
```

**Note:** Weather icons are generated dynamically from condition names using the OpenWeatherMap API.

### WeatherSearchChangeLog Table
```sql
CREATE TABLE [dbo].[WeatherSearchChangeLog](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [WeatherSearchId] [int] NOT NULL,
    [UserId] [int] NULL,
    [ChangeDate] [datetime] NOT NULL DEFAULT (getdate()),
    [ChangeType] [nvarchar](100) NOT NULL,
    [OldValue] [nvarchar](255) NULL,
    [NewValue] [nvarchar](255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
```

## Foreign Key Relationships

- **WeatherSearches.UserId** → **Users.Id**
- **WeatherSearchChangeLog.WeatherSearchId** → **WeatherSearches.Id**
- **WeatherSearchChangeLog.UserId** → **Users.Id**

## Sample Data

The schema includes sample users with the following credentials:
- **Username:** user1, user2, user3, user4, user5
- **Password:** 12345678 (for all users)

## Key Features

### Security
- **Password Hashing:** SHA2_256 encryption for all passwords
- **User Authentication:** Secure login with stored procedures
- **Session Management:** User state tracking via ASP.NET Core sessions

### Data Management
- **Weather Data:** Complete weather information storage
- **Change Tracking:** Comprehensive audit trail using WeatherSearchChangeLog
- **Dynamic Icons:** Weather icons generated on-the-fly from condition names
- **Pagination:** Efficient data retrieval with pagination support

### Performance
- **Indexed Queries:** Optimized database queries
- **Stored Procedures:** Pre-compiled SQL for better performance
- **Connection Pooling:** Efficient database connections

## Troubleshooting

### Common Issues

1. **Connection Failed**
   - Ensure LocalDB is running: `sqllocaldb start MSSQLLocalDB`
   - Verify server name: `(localdb)\MSSQLLocalDB`

2. **Authentication Errors**
   - Verify password hashing: Use SHA2_256
   - Check user credentials in Users table

3. **Missing Tables**
   - Execute the complete schema script
   - Verify all stored procedures are created

### Verification Queries

```sql
-- Check tables exist
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = 'dbo';

-- Check stored procedures exist
SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE' AND ROUTINE_SCHEMA = 'dbo';

-- Test user authentication
EXEC sp_ValidateUser 'user1', '12345678';
```

## Maintenance

### Regular Tasks
- **Backup:** Regular database backups
- **Index Maintenance:** Monitor and optimize indexes
- **Log Cleanup:** Archive old change logs if needed

### Updates
- **Schema Changes:** Use migration scripts
- **Data Migration:** Backup before major changes
- **Testing:** Verify functionality after updates

## Support

For database-related issues:
1. Check the troubleshooting section above
2. Verify all prerequisites are installed
3. Review the complete schema file for reference
4. Test with sample data provided

---

**Last Updated:** 2025-01-29  
**Version:** 2.1 (Icon column removed, redundant WeatherSearchChanges table removed, dynamic icon generation implemented) 