# Script PowerShell per applicare migration Entity Framework al database
param(
    [Parameter(Mandatory=$true)]
    [string]$ServiceName
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

Write-Host "Aggiornamento database per il servizio '$ServiceName'..."

# Vai alla directory del servizio
Push-Location $servicePath

try {
    # Esegui il comando dotnet ef database update
    $command = "dotnet ef database update --context $contextName"
    Write-Host "Eseguendo: $command"
    
    Invoke-Expression $command
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database aggiornato con successo!" -ForegroundColor Green
    } else {
        Write-Error "Errore durante l'aggiornamento del database"
    }
} finally {
    # Torna alla directory originale
    Pop-Location
}