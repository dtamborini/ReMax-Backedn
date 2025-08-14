# Sistema di Gestione Condomini - Microservizi

## Panoramica Architettura

Questo sistema è composto da 8 microservizi dedicati alla gestione completa di condomini, da preventivi a manutenzioni, gestione asset e utenti.

## Microservizi Implementati

### 1. **AuthenticationService** (Porta: 5001)
- **Scopo**: Gestione autenticazione e autorizzazione
- **Schema DB**: Nessuno (usa mock utenti)
- **Features**:
  - Login/Logout
  - Generazione JWT tokens
  - Modalità mock con 3 utenti predefiniti
  - Proxy verso servizio esterno di autenticazione

### 2. **BuildingService** (Porta: 5002)
- **Scopo**: Gestione edifici e proprietà
- **Schema DB**: `buildings`
- **Features**:
  - CRUD operazioni per edifici
  - Gestione informazioni strutturali
  - Tracking appartamenti e piani

### 3. **WorkQuoteService** (Porta: 5003)
- **Scopo**: Gestione preventivi di lavoro
- **Schema DB**: `work_quotes`
- **Features**:
  - Creazione e gestione preventivi
  - Stati: Draft, Sent, Approved, Rejected, Expired
  - Collegamento con edifici e contractors
  - Gestione scadenze e risposte

### 4. **WorkOrderService** (Porta: 5004)
- **Scopo**: Gestione ordini di lavoro
- **Schema DB**: `work_orders`
- **Features**:
  - Creazione ordini di lavoro
  - Gestione priorità e categorie
  - Stati: Open, Assigned, InProgress, Completed, Cancelled
  - Tracking costi stimati vs effettivi

### 5. **RfqService** (Porta: 5005)
- **Scopo**: Gestione richieste di offerta (Request for Quotation)
- **Schema DB**: `rfqs`
- **Features**:
  - Pubblicazione RFQ per lavori
  - Gestione scadenze submissions
  - Criteriali di valutazione
  - Selezione contractor vincitore

### 6. **WorkSheetService** (Porta: 5006)
- **Scopo**: Gestione fogli di lavoro e timesheet
- **Schema DB**: `work_sheets`
- **Features**:
  - Registrazione ore lavorate
  - Calcolo costi lavoro e materiali
  - Approvazione fogli di lavoro
  - Stati: Draft, Submitted, Approved, Rejected

### 7. **AssetService** (Porta: 5007)
- **Scopo**: Gestione asset e inventario
- **Schema DB**: `assets`
- **Features**:
  - Catalogazione asset (HVAC, ascensori, etc.)
  - Gestione manutenzioni programmate
  - Tracking garanzie e ispezioni
  - Valutazione asset nel tempo

### 8. **UsersService** (Porta: 5008)
- **Scopo**: Gestione utenti e profili
- **Schema DB**: `users`
- **Features**:
  - Gestione utenti (admin, residents, contractors)
  - Profili con informazioni dettagliate
  - Gestione ruoli e permessi
  - Informazioni aziendali per contractors

### 9. **AttachmentService** (Porta: 5009)
- **Scopo**: Gestione file e allegati
- **Schema DB**: `attachments`
- **Features**:
  - Upload/download file
  - Collegamento allegati a qualsiasi entità
  - Gestione thumbnail e metadati
  - Controllo accessi e scadenze

## Architettura Condivisa

### **CondominiumManagement.Shared**
Libreria condivisa che fornisce:
- **JWT Authentication**: Middleware per validazione token
- **Database Extensions**: Configurazione PostgreSQL automatica
- **BaseEntity**: Entità base con soft delete e audit trail
- **BaseDbContext**: Context base con funzionalità comuni

## Database PostgreSQL

### Configurazione
- **Host**: localhost:5432
- **Database**: condominium_management
- **User**: condominium_user
- **Schemi separati** per ogni microservizio

### Schemi Database
```sql
-- Ogni microservizio ha il proprio schema
buildings         -- BuildingService
work_quotes      -- WorkQuoteService  
work_orders      -- WorkOrderService
rfqs             -- RfqService
work_sheets      -- WorkSheetService
assets           -- AssetService
users            -- UsersService
attachments      -- AttachmentService
```

## Docker Compose

### Servizi Configurati
- **postgres-db**: PostgreSQL 16 con schemi pre-configurati
- **authentication-service**: Porta 5001
- **building-service**: Porta 5002
- **workquote-service**: Porta 5003
- **workorder-service**: Porta 5004
- **rfq-service**: Porta 5005
- **worksheet-service**: Porta 5006
- **asset-service**: Porta 5007
- **users-service**: Porta 5008
- **attachment-service**: Porta 5009

### Comandi Docker
```bash
# Avvia tutti i servizi
docker-compose up -d

# Avvia solo PostgreSQL
docker-compose up -d postgres-db

# Avvia un servizio specifico
docker-compose up -d building-service
```

## Sicurezza e Autenticazione

### JWT Configuration
- **Algoritmo**: HMAC SHA256
- **Validazione**: Solo SecretKey (no Issuer/Audience)
- **Scadenza Access Token**: 60 minuti
- **Scadenza Refresh Token**: 7 giorni

### Utenti Mock (AuthenticationService)
- **admin**: Administrator role - Mario Rossi
- **supplier**: Supplier role - Giuseppe Verdi  
- **resident**: Resident role - Anna Bianchi

## Entity Framework e Migration

### Comandi Migration
```bash
# Creare migration per un servizio
cd src[ServiceName]/[ServiceName]
dotnet ef migrations add [MigrationName] --context [ServiceName]DbContext

# Applicare migration
dotnet ef database update --context [ServiceName]DbContext
```

### Tools Disponibili
- `tools/create-migration.ps1`: Script per creare migration
- `tools/update-database.ps1`: Script per applicare migration

## Sviluppo e Testing

### Build Solution
```bash
dotnet build
```

### Health Checks
Ogni servizio espone un endpoint `/health`:
- http://localhost:5001/health (Authentication)
- http://localhost:5002/health (Building)
- http://localhost:5003/health (WorkQuote)
- etc...

### OpenAPI/Swagger
In modalità Development, ogni servizio espone OpenAPI su `/openapi/v1.json`

## Pattern e Best Practices

### Struttura Microservizi
- **Program.cs**: Configurazione servizio e middleware
- **Data/Entities**: Entità del dominio
- **Data/Context**: DbContext specifico
- **Controllers**: API endpoints (da implementare)

### Convenzioni Database
- **Soft Delete**: Tutte le entità supportano soft delete
- **Audit Trail**: CreatedAt, UpdatedAt automatici
- **GUID**: Chiavi primarie con Guid
- **Schema Separation**: Un schema per microservizio

### Cross-Service Communication
- **Eventi**: Da implementare per comunicazione asincrona
- **API Gateway**: Da considerare per routing centralizzato
- **Service Discovery**: Da implementare per ambienti production

## Prossimi Sviluppi

1. **Controller Implementation**: Implementare controller REST per ogni servizio
2. **Business Logic**: Aggiungere repository pattern e servizi business
3. **API Gateway**: Centralizzare routing e autenticazione
4. **Event Sourcing**: Implementare eventi per comunicazione inter-servizi
5. **Monitoring**: Aggiungere logging e telemetria
6. **Testing**: Unit e integration tests
7. **CI/CD**: Pipeline di deployment automatico

## Note Tecniche

- **.NET 9.0**: Framework utilizzato
- **PostgreSQL 16**: Database relazionale
- **Entity Framework Core 9.0**: ORM
- **JWT Bearer Authentication**: Autenticazione
- **Docker**: Containerizzazione
- **OpenAPI/Swagger**: Documentazione API