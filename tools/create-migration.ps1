# Script PowerShell per creare migration Entity Framework
param(
    [Parameter(Mandatory=$true)]
    [string]$ServiceName,
    
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

$servicePath = ""
$contextName = ""

switch ($ServiceName.ToLower()) {
    "building" { 
        $servicePath = "srcBuildingService\BuildingService"
        $contextName = "BuildingDbContext"
    }
    "authentication" { 
        $servicePath = "srcAuthenticationService\AuthenticationService"
        $contextName = "AuthDbContext"
    }
    default {
        Write-Error "Servizio non riconosciuto: $ServiceName"
        Write-Host "Servizi disponibili: building, authentication"
        exit 1
    }
}

if (-not (Test-Path $servicePath)) {
    Write-Error "Percorso servizio non trovato: $servicePath"
    exit 1
}

Write-Host "Creazione migration '$MigrationName' per il servizio '$ServiceName'..."

# Vai alla directory del servizio
Push-Location $servicePath

try {
    # Esegui il comando dotnet ef migrations add
    $command = "dotnet ef migrations add $MigrationName --context $contextName"
    Write-Host "Eseguendo: $command"
    
    Invoke-Expression $command
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration creata con successo!" -ForegroundColor Green
        Write-Host "Per applicare la migration al database, eseguire:" -ForegroundColor Yellow
        Write-Host "  dotnet ef database update --context $contextName" -ForegroundColor Yellow
    } else {
        Write-Error "Errore durante la creazione della migration"
    }
} finally {
    # Torna alla directory originale
    Pop-Location
}