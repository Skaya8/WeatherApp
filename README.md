# WeatherApp

A modern weather application built with ASP.NET Core 8.0 that provides real-time weather data, search functionality, and comprehensive history management.

## Overview

WeatherApp is a web-based application that allows users to search for weather information across 27 major cities worldwide, save search results, and maintain a detailed history of weather queries. The application integrates with the OpenWeatherMap API to provide accurate, real-time weather data.

## Features

- Real-time weather data retrieval from OpenWeatherMap API
- Support for 27 major cities worldwide including Riyadh, London, New York, Tokyo, and more
- Comprehensive weather information display (temperature, humidity, wind speed, conditions)
- User authentication and session management (with secure password hashing)
- Weather search history with filtering and search capabilities
- Edit functionality for saved weather records
- Responsive design optimized for desktop and mobile devices
- Modern UI with Lottie animations for enhanced user experience

## Prerequisites

Before running this application, ensure you have the following software installed:

- **Visual Studio 2022** (Community, Professional, or Enterprise) - [Download](https://visualstudio.microsoft.com/downloads/)
- **SQL Server Management Studio (SSMS)** - [Download](https://aka.ms/ssms)
- **SQL Server LocalDB** (included with Visual Studio installation)

## Installation and Setup

### Step 1: Project Initialization

1. Download and extract the project files to your local development environment
2. Open the project using one of the following methods:
   - Double-click `open_in_visual_studio.bat` to launch directly in Visual Studio
   - Open `WeatherApp.csproj` manually in Visual Studio 2022

### Step 2: Database Configuration

1. Launch SQL Server Management Studio (SSMS)
2. Connect to LocalDB using the following parameters:
   - **Server name:** `(localdb)\MSSQLLocalDB`
   - **Authentication:** Windows Authentication
3. Create a new database:
   - Right-click on "Databases" in Object Explorer
   - Select "New Database"
   - Name the database `WeatherAppDB`
   - Click "OK" to create
4. **Recommended:** Open and execute the complete script `Database/WeatherAppDB_Complete_Schema.sql` to create all tables, stored procedures, and test users with secure password hashes.
5. **Manual Option:** See `Database/README.md` for step-by-step SQL scripts and explanations.
6. **Test User Creation (Secure):**
   ```sql
   -- If you need to add more users, use this format:
   INSERT INTO Users (Username, PasswordHash) VALUES ('newuser', HASHBYTES('SHA2_256', 'yourpassword'));
   GO
   ```

### Step 3: Application Execution

1. In Visual Studio 2022:
   - Build the solution using `Ctrl+Shift+B` or Build → Build Solution
   - Run the application using `F5` or the Start button
2. The application will launch at:
   - Primary URL: `https://localhost:7261`
   - Alternative URL: `http://localhost:5043`
3. Access the application using the provided test credentials:
   - **Username:** `user1` (or any configured user account)
   - **Password:** `12345678`

## Architecture

The application follows the Model-View-Controller (MVC) architectural pattern and implements SOLID principles:

### Project Structure
```
WeatherApp/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── HistoryController.cs
│   └── WeatherController.cs
├── Models/              # Data Models
│   ├── LoginViewModel.cs
│   ├── WeatherSearchResult.cs
│   └── WeatherViewModel.cs
├── Services/            # Business Logic Layer
│   ├── Interfaces/      # Service Contracts
│   ├── Repositories/    # Data Access Layer
│   └── WeatherService.cs
├── Views/               # Razor Views
│   ├── Account/
│   ├── History/
│   ├── Shared/
│   └── Weather/
├── wwwroot/             # Static Assets
│   ├── css/
│   ├── js/
│   └── lottie/
├── Database/            # Database Scripts
│   └── WeatherAppDB_Complete_Schema.sql
├── Program.cs           # Application Entry Point
├── appsettings.json     # Configuration
└── WeatherApp.csproj    # Project File
```

### Design Patterns

- **Repository Pattern:** Abstracts data access logic
- **Dependency Injection:** Manages service dependencies
- **Interface Segregation:** Separates concerns through focused interfaces
- **Single Responsibility:** Each class has a single, well-defined purpose

## Configuration

### API Configuration
The application integrates with OpenWeatherMap API for weather data retrieval. The API key is pre-configured for demonstration purposes.

### Database Configuration
The default connection string in `appsettings.json` is configured for LocalDB:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=WeatherAppDB;Trusted_Connection=True;"
  }
}
```

## Troubleshooting

### Common Issues and Solutions

**Port Conflict Resolution**
- If port 5043 or 7261 is already in use, execute `fix_port_issues.bat` to automatically resolve conflicts. The script now checks both ports and helps you kill any locked processes.
- Alternatively, manually terminate processes using the port or modify `Properties/launchSettings.json`.

**Database Connection Issues**
- Verify LocalDB is properly installed and running
- Confirm the connection string in `appsettings.json` matches your database configuration
- Ensure all SQL initialization scripts have been executed successfully

**Build Errors**
- Clean the solution (Build → Clean Solution)
- Rebuild the solution (Build → Rebuild Solution)
- Verify all NuGet packages are properly restored

**Authentication Issues**
- Ensure passwords are inserted using SHA2_256 hashing as shown above
- Use the provided stored procedure `sp_ValidateUser` for secure login

## Technology Stack

- **Backend Framework:** ASP.NET Core 8.0
- **Programming Language:** C#
- **Database:** SQL Server LocalDB
- **Frontend:** HTML5, CSS3, JavaScript
- **UI Framework:** Bootstrap 5.3
- **JavaScript Libraries:** jQuery, jQuery Validation
- **Animations:** Lottie Web
- **External API:** OpenWeatherMap
- **Development Environment:** Visual Studio 2022

## Additional Documentation

For detailed setup instructions with screenshots and advanced configuration options, refer to:
- [VISUAL_STUDIO_SETUP.md](VISUAL_STUDIO_SETUP.md) - Comprehensive Visual Studio setup guide
- [Database/README.md](Database/README.md) - Database schema and initialization scripts
- [Database/WeatherAppDB_Complete_Schema.sql](Database/WeatherAppDB_Complete_Schema.sql) - Full database setup script

## License

This project is developed for educational and demonstration purposes. All rights reserved.

## Support

For technical support or questions regarding this application, please refer to the troubleshooting section above or consult the additional documentation files included in the project. 