#!/bin/bash

echo "========================================"
echo "WeatherApp Setup Script"
echo "========================================"
echo

echo "Checking for .NET 8.0 SDK..."
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET 8.0 SDK is not installed!"
    echo "Please download and install from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

echo ".NET SDK found. Version:"
dotnet --version
echo

echo "Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to restore packages!"
    exit 1
fi

echo "Building the project..."
dotnet build
if [ $? -ne 0 ]; then
    echo "ERROR: Build failed!"
    exit 1
fi

echo
echo "========================================"
echo "Setup completed successfully!"
echo "========================================"
echo
echo "To run the application:"
echo "1. Run: dotnet run"
echo "2. Open your browser to: https://localhost:5001"
echo
echo "Note: Make sure SQL Server is installed and configured."
echo "For development, you can use SQL Server LocalDB or SQL Server Express."
echo 