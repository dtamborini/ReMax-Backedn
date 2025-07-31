# Guida Migrazione a Shared Project

Questa guida spiega come migrare i microservizi per utilizzare il progetto shared `RemaxApi.Shared.Authentication`.

## 🏗️ **Struttura Shared Project**

```
Shared/
└── RemaxApi.Shared.Authentication/
    ├── RemaxApi.Shared.Authentication.csproj
    ├── README.md
    ├── Middleware/
    │   └── JwtValidationMiddleware.cs
    ├── Services/
    │   └── ExternalAuthUserService.cs
    └── Extensions/
        ├── ServiceCollectionExtensions.cs
        └── ApplicationBuilderExtensions.cs
```

## 🔄 **Come Migrare un Microservizio**

### 1. **Aggiungi Project Reference**

Nel file `.csproj` del microservizio:

```xml
<ItemGroup>
  <ProjectReference Include="..\Shared\RemaxApi.Shared.Authentication\RemaxApi.Shared.Authentication.csproj" />
</ItemGroup>
```

### 2. **Aggiorna Program.cs**

```csharp
// PRIMA (da rimuovere)
using AttachmentService.Middleware;
using AttachmentService.Services;
builder.Services.AddScoped<IExternalAuthUserService, ExternalAuthUserService>();
app.UseJwtValidation();

// DOPO (da aggiungere)
using RemaxApi.Shared.Authentication.Extensions;
builder.Services.AddExternalJwtAuthentication();
app.UseExternalJwtValidation();
```

### 3. **Aggiorna i Controllers**

```csharp
// PRIMA (da rimuovere)
using AttachmentService.Services;

// DOPO (da aggiungere)  
using RemaxApi.Shared.Authentication.Services;
```

### 4. **Rimuovi File Locali**

Elimina questi file dal microservizio (ora sono nel shared project):
- `Middleware/JwtValidationMiddleware.cs`
- `Services/ExternalAuthUserService.cs`

### 5. **Mantieni Configurazione**

Il file `appsettings.json` rimane uguale:
```json
{
  "ExternalAuth": {
    "SecretKey": "external-company-shared-secret-key-1234567890-very-long-and-secure",
    "Issuer": "external-company-auth-service", 
    "Audience": "remax-microservices"
  }
}
```

## ✅ **Esempio Completo: AttachmentService**

### AttachmentService.csproj
```xml
<ItemGroup>
  <ProjectReference Include="..\Shared\RemaxApi.Shared.Authentication\RemaxApi.Shared.Authentication.csproj" />
</ItemGroup>
```

### Program.cs
```csharp
using RemaxApi.Shared.Authentication.Extensions;

// Registra servizi
builder.Services.AddExternalJwtAuthentication();

// Configura middleware
app.UseExternalJwtValidation();
app.UseAuthentication();
app.UseAuthorization();
```

### Controllers/AttachmentQuoteController.cs
```csharp
using RemaxApi.Shared.Authentication.Services;

public class AttachmentQuoteController : ControllerBase
{
    private readonly IExternalAuthUserService _externalAuthUserService;
    
    public AttachmentQuoteController(IExternalAuthUserService externalAuthUserService)
    {
        _externalAuthUserService = externalAuthUserService;
    }
}
```

## 📋 **Lista Microservizi da Migrare**

- [x] **AttachmentService** ✅ Completato
- [ ] **UserService**  
- [ ] **BuildingService**
- [ ] **AssetService**
- [ ] **QuoteService**
- [ ] **RfqService**
- [ ] **WorkOrderService**
- [ ] **WorkSheetService**

## 🔄 **Script di Migrazione Veloce**

Per ogni microservizio, esegui questi comandi:

```bash
# 1. Aggiungi reference
dotnet add [MicroserviceName] reference ../Shared/RemaxApi.Shared.Authentication/RemaxApi.Shared.Authentication.csproj

# 2. Rimuovi file duplicati (se esistono)
rm [MicroserviceName]/Middleware/JwtValidationMiddleware.cs
rm [MicroserviceName]/Services/ExternalAuthUserService.cs
```

Poi aggiorna manualmente Program.cs e i Controllers.

## 🎯 **Vantaggi della Migrazione**

1. **Codice unico**: Un posto solo per middleware e servizi JWT
2. **Aggiornamenti centrali**: Modifiche si propagano a tutti i servizi
3. **Coerenza**: Comportamento identico su tutti i microservizi
4. **Manutenzione ridotta**: Meno duplicazione = meno bug

## 🐛 **Risoluzione Problemi**

### Build Error "Reference not found"
- Verifica che il path del ProjectReference sia corretto
- Controlla che il progetto shared sia incluso nella solution

### Namespace non trovati
- Aggiungi `using RemaxApi.Shared.Authentication.Extensions;`
- Aggiungi `using RemaxApi.Shared.Authentication.Services;`

### Middleware non funziona
- Verifica che `UseExternalJwtValidation()` sia chiamato prima di `UseAuthentication()`
- Controlla la configurazione `ExternalAuth` in appsettings.json