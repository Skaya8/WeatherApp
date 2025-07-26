@echo off
echo ========================================
echo Opening WeatherApp in Visual Studio
echo ========================================
echo.

echo Checking if Visual Studio is installed...
where devenv >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Visual Studio is not installed or not in PATH!
    echo Please install Visual Studio 2022 from: https://visualstudio.microsoft.com/downloads/
    echo Make sure to include "ASP.NET and web development" workload.
    pause
    exit /b 1
)

echo Visual Studio found. Opening project...
devenv WeatherApp.csproj

echo.
echo ========================================
echo Project opened in Visual Studio!
echo ========================================
echo.
echo Next steps:
echo 1. Build the solution (Ctrl+Shift+B)
echo 2. Run the application (F5)
echo 3. Make sure the database is set up in SSMS
echo.
pause 