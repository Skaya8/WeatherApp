@echo off
echo ========================================
echo WeatherApp Setup Script
echo ========================================
echo.

echo Checking for .NET 8.0 SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET 8.0 SDK is not installed!
    echo Please download and install from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo .NET SDK found. Version:
dotnet --version
echo.

echo Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages!
    pause
    exit /b 1
)

echo Building the project...
dotnet build
if %errorlevel% neq 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Setup completed successfully!
echo ========================================
echo.
echo To run the application:
echo 1. Run: dotnet run
echo 2. Open your browser to: https://localhost:5001
echo.
echo Note: Make sure SQL Server LocalDB is installed.
echo If you don't have it, install Visual Studio or SQL Server Express.
echo.
pause 