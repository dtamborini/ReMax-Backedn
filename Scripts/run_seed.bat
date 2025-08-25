@echo off
echo Starting data seeding script...
cd /d "%~dp0"
dotnet restore
dotnet run
pause