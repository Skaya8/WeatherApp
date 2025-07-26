@echo off
echo ========================================
echo WeatherApp Port Conflict Fixer
echo ========================================
echo.

echo Checking for processes using port 5043...
netstat -ano | findstr :5043
if %errorlevel% equ 0 (
    echo.
    echo Found processes using port 5043:
    netstat -ano | findstr :5043
    echo.
    echo Checking for WeatherApp processes...
    tasklist | findstr WeatherApp
    if %errorlevel% equ 0 (
        echo.
        echo Found WeatherApp processes. Killing them...
        taskkill /IM WeatherApp.exe /F
        echo WeatherApp processes killed.
    ) else (
        echo No WeatherApp processes found.
    )
    echo.
    echo Checking for dotnet processes that might be using the port...
    tasklist | findstr dotnet
    echo.
    echo If you see dotnet processes above, you may need to kill them manually:
    echo taskkill /PID [PID_NUMBER] /F
) else (
    echo No processes found using port 5043.
)

echo.
echo ========================================
echo Port check complete!
echo ========================================
echo.
echo Now you can run the application:
echo 1. In Visual Studio: Press F5
echo 2. Or command line: dotnet run
echo.
echo The application will run on:
echo - HTTP:  http://localhost:5043
echo - HTTPS: https://localhost:7261
echo.
pause 