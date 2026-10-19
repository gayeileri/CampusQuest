@echo off
REM CampusQuest Mobile - Build Clean Script
REM This script cleans all build artifacts and rebuilds the project

echo ========================================
echo CampusQuest Mobile - Cleaning & Building
echo ========================================

REM Navigate to project directory
cd /d "%~dp0"

REM Clean bin and obj folders
echo.
echo [1/4] Removing build artifacts...
for /r . %%A in (bin) do (
	if exist "%%A" (
		echo Removing %%A
		rmdir /s /q "%%A"
	)
)

for /r . %%A in (obj) do (
	if exist "%%A" (
		echo Removing %%A
		rmdir /s /q "%%A"
	)
)

echo.
echo [2/4] Clearing NuGet cache...
nuget locals all -clear

echo.
echo [3/4] Restoring NuGet packages...
nuget restore "CampusQuest.Mobile\CampusQuest.Mobile.csproj"

echo.
echo [4/4] Building solution...
dotnet build CampusQuest.Mobile/CampusQuest.Mobile.csproj -c Debug -f net8.0-android

echo.
echo ========================================
echo Build Complete!
echo ========================================
echo.
echo To run on Android emulator:
echo   dotnet maui run -f net8.0-android
echo.
pause
