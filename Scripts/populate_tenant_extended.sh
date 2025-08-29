#!/bin/bash

# Script per popolare un tenant con dati di test estesi (incluse relazioni)
# Uso: ./populate_tenant_extended.sh [nome_tenant]

set -e

# Parametri database
DB_HOST="localhost"
DB_PORT="5433"
DB_NAME="remax_management"
DB_USER="remax_user"
DB_PASS="remax_password123"

# Colori per output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Funzione per stampare messaggi colorati
print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Verifica parametri
if [ $# -eq 0 ]; then
    print_error "Uso: $0 [nome_tenant]"
    print_info "Esempio: $0 test_extended"
    exit 1
fi

TENANT_NAME="$1"
TENANT_SCHEMA="tenant_${TENANT_NAME}"
SCRIPT_DIR="$(dirname "$0")"
SQL_FILE="${SCRIPT_DIR}/populate_tenant_data_extended_simple.sql"

# Verifica che il file SQL esista
if [ ! -f "$SQL_FILE" ]; then
    print_error "File SQL non trovato: $SQL_FILE"
    exit 1
fi

print_info "Popolamento tenant esteso: $TENANT_NAME"
print_info "Schema database: $TENANT_SCHEMA"

# Esegue lo script SQL con il search_path corretto
print_info "Esecuzione dello script SQL esteso..."

# Comando psql con search_path impostato
PGPASSWORD="$DB_PASS" psql -h "$DB_HOST" -p "$DB_PORT" -d "$DB_NAME" -U "$DB_USER" \
    -c "SET search_path TO '${TENANT_SCHEMA}';" \
    -f "$SQL_FILE"

if [ $? -eq 0 ]; then
    print_success "Dati estesi inseriti con successo nel tenant '$TENANT_NAME'!"
    print_info "Schema utilizzato: $TENANT_SCHEMA"
    
    # Mostra un riepilogo
    echo
    print_info "🎯 Riepilogo dati inseriti (ESTESO):"
    echo "📁 3 Buildings (Extended versions)"
    echo "🏗️  3 Suppliers (2 approvati, 1 in attesa)"
    echo "🔗 6 Relazioni Supplier-Building"
    echo "🏠 9 Premises (3 appartamenti per building)"
    echo "👤 3 Residents (Extended versions)"  
    echo "🏡 5 Relazioni Resident-Premises (inclusa comproprietà)"
    echo
    print_info "📊 Struttura relazioni create:"
    echo "   • Mario: proprietà esclusiva (100%) + comproprietà (50%) attico"
    echo "   • Anna: proprietà esclusiva (100%) + comproprietà (50%) attico"  
    echo "   • Franco: proprietà maggioritaria (80%) penthouse di lusso"
    echo "   • Edilservice: supplier preferito per 1 building, lavora per tutti e 3"
    echo "   • Idraulica: supplier preferito per 1 building, lavora per 2"
    echo "   • Elettrica: in attesa approvazione, già assegnata a 1 building"
else
    print_error "Errore durante l'esecuzione dello script SQL esteso"
    exit 1
fi