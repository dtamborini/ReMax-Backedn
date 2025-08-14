# Script PowerShell per testare la connessione PostgreSQL
param(
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString = "Host=localhost;Database=condominium_management;Username=condominium_user;Password=condominium_password123;Port=5432"
)

Write-Host "Testing PostgreSQL connection..."
Write-Host "Connection string: $ConnectionString"

Add-Type -Path "C:\Users\david\.nuget\packages\npgsql\9.0.4\lib\net8.0\Npgsql.dll"

try {
    $connection = New-Object Npgsql.NpgsqlConnection($ConnectionString)
    $connection.Open()
    
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT version();"
    $result = $command.ExecuteScalar()
    
    Write-Host "SUCCESS: Connected to PostgreSQL!" -ForegroundColor Green
    Write-Host "Version: $result" -ForegroundColor Green
    
    $connection.Close()
} catch {
    Write-Host "ERROR: Failed to connect to PostgreSQL" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Inner Exception: $($_.Exception.InnerException.Message)" -ForegroundColor Red
}