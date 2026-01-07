#!/bin/sh
set -e

echo "Waiting for database to be ready..."
cd /src/SDMISAppQG

# Restore dependencies first
echo "Restoring dependencies..."
dotnet restore

# Apply migrations
until dotnet ef database update 2>/dev/null; do
  echo "Database not ready, retrying in 2 seconds..."
  sleep 2
done

echo "Migrations applied successfully!"
echo "Starting application..."
cd /app
exec dotnet SDMISAppQG.dll
