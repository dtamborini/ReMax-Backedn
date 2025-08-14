# Sistema di Gestione Condomini - Microservizi

Sistema basato su microservizi per la gestione di condomini sviluppato in .NET.

## Architettura

Il sistema è composto da due microservizi principali:

### 1. AuthenticationService (Porta 5001)
Gestisce l'autenticazione e autorizzazione tramite JWT token.

**Endpoints:**
- `POST /auth/login` - Login utente
- `GET /auth/validate` - Validazione token (richiede autenticazione)

**Login di test:**
- Username: `admin`
- Password: `password`

### 2. BuildingService (Porta 5002) 
Gestisce i dati dei condomini. Tutti gli endpoint richiedono autenticazione JWT.

**Endpoints:**
- `GET /buildings` - Ottiene tutti i condomini
- `GET /buildings/{id}` - Ottiene un condominio specifico
- `POST /buildings` - Crea un nuovo condominio
- `PUT /buildings/{id}` - Aggiorna un condominio esistente
- `DELETE /buildings/{id}` - Elimina un condominio

## Come Avviare

### Opzione 1: Docker Compose (Consigliato)
```bash
docker-compose up --build
```

### Opzione 2: Esecuzione Manuale
```bash
# Terminal 1 - AuthenticationService
cd srcAuthenticationService/AuthenticationService
dotnet run

# Terminal 2 - BuildingService  
cd srcBuildingService/BuildingService
dotnet run
```

## Testing delle API

### 1. Ottenere un token di autenticazione:
```bash
curl -X POST http://localhost:5001/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'
```

### 2. Utilizzare il token per accedere ai condomini:
```bash
curl -X GET http://localhost:5002/buildings \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 3. Creare un nuovo condominio:
```bash
curl -X POST http://localhost:5002/buildings \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"name":"Nuovo Condominio","address":"Via Test 123","yearBuilt":2023,"numberOfUnits":20}'
```

## Struttura del Progetto

```
CondominiumManagement/
├── CondominiumManagement.sln
├── docker-compose.yml
├── srcAuthenticationService/
│   └── AuthenticationService/
│       ├── AuthenticationService.csproj
│       ├── Program.cs
│       ├── Dockerfile
│       └── appsettings.json
└── srcBuildingService/
    └── BuildingService/
        ├── BuildingService.csproj
        ├── Program.cs
        ├── Dockerfile
        └── appsettings.json
```

## Prossimi Sviluppi

Il sistema è progettato per essere esteso facilmente con nuovi microservizi:
- ResidentService (gestione inquilini)
- MaintenanceService (gestione manutenzioni)
- ExpenseService (gestione spese condominiali)
- NotificationService (notifiche)

## Note di Sicurezza

- Il JWT key utilizzato è solo per demo. In produzione utilizzare un key sicuro e gestirlo tramite variabili d'ambiente
- Implementare un sistema di autenticazione più robusto con database utenti
- Aggiungere HTTPS in produzione