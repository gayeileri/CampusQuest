#!/bin/bash
# CampusQuest Mobile - Build & Run Script for Android

echo "========================================"
echo "CampusQuest Mobile - Building for Android"
echo "========================================"
echo ""

# Navigate to project directory
cd "$(dirname "$0")"

# Clean previous builds
echo "[1/5] Removing build artifacts..."
find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null || true
find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null || true

echo "[2/5] Restoring NuGet packages..."
dotnet restore CampusQuest.Mobile/CampusQuest.Mobile.csproj

echo "[3/5] Building solution..."
dotnet build CampusQuest.Mobile/CampusQuest.Mobile.csproj -c Debug -f net8.0-android

if [ $? -eq 0 ]; then
	echo ""
	echo "[4/5] Build successful!"
	echo "[5/5] Launching on Android emulator/device..."
	echo ""
	dotnet maui run -f net8.0-android
else
	echo ""
	echo "❌ Build failed! Check errors above."
	exit 1
fi
