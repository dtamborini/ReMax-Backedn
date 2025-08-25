#!/bin/bash
echo "Starting data seeding script..."
cd "$(dirname "$0")"
dotnet restore
dotnet run