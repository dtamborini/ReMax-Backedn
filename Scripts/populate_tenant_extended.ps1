# Script PowerShell per popolare un tenant con dati di test estesi (incluse relazioni)
# Uso: .\populate_tenant_extended.ps1 [nome_tenant]

param(
    [Parameter(Mandatory=$true)]
    [string]$TenantName
)

# Parametri database
$DB_HOST = "localhost"
$DB_PORT = "5433"
$DB_NAME = "remax_management"
$DB_USER = "remax_user"
$DB_PASS = "remax_password123"

# Funzioni per output colorato
function Write-Info {
    param([string]$Message)
    Write-Host "INFO: $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "SUCCESS: $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "WARNING: $Message" -ForegroundColor Yellow
}

function Write-CustomError {
    param([string]$Message)
    Write-Host "ERROR: $Message" -ForegroundColor Red
}

$TENANT_SCHEMA = "tenant_$TenantName"
$SCRIPT_DIR = Split-Path -Parent $MyInvocation.MyCommand.Path
$SQL_FILE = Join-Path $SCRIPT_DIR "populate_tenant_data_extended_simple.sql"

# Verifica che il file SQL esista
if (!(Test-Path $SQL_FILE)) {
    Write-CustomError "File SQL non trovato: $SQL_FILE"
    exit 1
}

Write-Info "Popolamento tenant esteso: $TenantName"
Write-Info "Schema database: $TENANT_SCHEMA"

# Esegue lo script SQL con il search_path corretto
Write-Info "Esecuzione dello script SQL esteso..."

# Imposta la password come variabile d'ambiente
$env:PGPASSWORD = $DB_PASS

# Comando psql con search_path impostato
try {
    $result = & psql -h $DB_HOST -p $DB_PORT -d $DB_NAME -U $DB_USER -c "SET search_path TO '$TENANT_SCHEMA';" -f $SQL_FILE 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Dati estesi inseriti con successo nel tenant '$TenantName'!"
        Write-Info "Schema utilizzato: $TENANT_SCHEMA"
        
        # Mostra un riepilogo
        Write-Host ""
        Write-Info "Riepilogo dati inseriti (ESTESO):"
        Write-Host "- 3 Buildings (Extended versions)"
        Write-Host "- 3 Suppliers (2 approvati, 1 in attesa)"
        Write-Host "- 6 Relazioni Supplier-Building"
        Write-Host "- 9 Premises (3 appartamenti per building)"
        Write-Host "- 3 Residents (Extended versions)"
        Write-Host "- 5 Relazioni Resident-Premises (inclusa comproprietà)"
        Write-Host ""
        Write-Info "Struttura relazioni create:"
        Write-Host "   Mario: proprietà esclusiva (100%) + comproprietà (50%) attico"
        Write-Host "   Anna: proprietà esclusiva (100%) + comproprietà (50%) attico"
        Write-Host "   Franco: proprietà maggioritaria (80%) penthouse di lusso"
        Write-Host "   Edilservice: supplier preferito per 1 building, lavora per tutti e 3"
        Write-Host "   Idraulica: supplier preferito per 1 building, lavora per 2"
        Write-Host "   Elettrica: in attesa approvazione, già assegnata a 1 building"
    } else {
        Write-CustomError "Errore durante l'esecuzione dello script SQL esteso"
        Write-Host $result
        exit 1
    }
} catch {
    Write-CustomError "Errore durante l'esecuzione: $($_.Exception.Message)"
    exit 1
} finally {
    # Rimuove la password dalla variabile d'ambiente
    Remove-Item env:PGPASSWORD -ErrorAction SilentlyContinue
}