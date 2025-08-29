# Script per Popolamento Tenant

Questi script permettono di popolare un tenant con dati di test per le tabelle principali e le loro relazioni.

## File disponibili

### Script Completo (con relazioni)
- `populate_tenant_data_extended_simple.sql` - Script SQL completo con relazioni
- `populate_tenant_extended.sh` - Script bash per Linux/Mac
- `populate_tenant_extended.ps1` - Script PowerShell per Windows

## Dati inseriti

### Script Completo

#### Buildings (3 elementi)
- **Condominio Bella Vista Extended** - Via Roma 123, Milano
- **Residence Le Torri Extended** - Corso Venezia 45, Milano  
- **Palazzo Reale Extended** - Piazza del Duomo 1, Milano

#### Suppliers (3 elementi)
- **Edilservice Milano Extended SRL** - Servizi manutenzione (Approvato)
- **Idraulica Lombarda Extended SNC** - Impianti idraulici (Approvato)
- **Elettrica Nord Extended SRL** - Impianti elettrici (In attesa approvazione)

#### Residents (3 elementi)
- **Mario Condomini Extended** - mario.extended@email.it
- **Anna Palazzi Extended** - anna.extended@email.it
- **Franco Abitanti Extended** - franco.extended@email.it

#### SupplierBuildings (6 relazioni)
- Edilservice → Tutti e 3 i buildings (favorito per Bella Vista)
- Idraulica → 2 buildings (favorito per Le Torri)  
- Elettrica → 1 building (favorito per Palazzo Reale, in attesa approvazione)

#### PremisesBuildings (9 premises)
- **Bella Vista**: 3 appartamenti (piano terra, primo piano, attico)
- **Le Torri**: 3 appartamenti (bilocale, trilocale, quadrilocale)
- **Palazzo Reale**: 3 appartamenti di lusso (rappresentanza, suite, penthouse)

#### ResidentPremises (5 relazioni)
- **Mario**: proprietà esclusiva (100%) appartamento + comproprietà (50%) attico
- **Anna**: proprietà esclusiva (100%) bilocale + comproprietà (50%) attico  
- **Franco**: proprietà maggioritaria (80%) penthouse di lusso

## Uso

### Windows (PowerShell)
```powershell
cd scripts
.\populate_tenant_extended.ps1 test6
```

### Linux/Mac (Bash)
```bash
cd scripts
./populate_tenant_extended.sh test6
```

### Diretto con psql
```bash
# Imposta il search path e esegui lo script
psql -h localhost -p 5433 -d remax_management -U remax_user \
  -c "SET search_path TO 'tenant_test6';" \
  -f populate_tenant_data_extended_simple.sql
```

## Prerequisiti

- PostgreSQL client (`psql`) installato
- Accesso al database remax_management
- Tenant già creato (con schema esistente)

## Note

- Gli script utilizzano `gen_random_uuid()` per generare ID univoci
- I dati sono di esempio e possono essere modificati nel file SQL
- Assicurati che il tenant esista prima di eseguire lo script
- Gli script sono sicuri da eseguire più volte (potrebbero generare errori di duplicati se gli ID sono già esistenti)
- Lo script completo include tutte le relazioni necessarie per test completi del sistema