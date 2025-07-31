# RemaxApi.Shared.Authentication

Libreria condivisa per l'autenticazione JWT tra i microservizi della piattaforma REMAX.

## 🎯 Scopo

Questa libreria fornisce componenti comuni per validare e gestire i token JWT emessi dal servizio di login dell'azienda partner, consentendo l'autenticazione unificata attraverso tutti i microservizi.

## 📦 Componenti Inclusi

### Middleware
- **JwtValidationMiddleware**: Valida automaticamente i token JWT nelle richieste HTTP

### Servizi
- **IExternalAuthUserService**: Interfaccia per accedere ai dati dell'utente autenticato
- **ExternalAuthUserService**: Implementazione che estrae claims dai token JWT

### Extensions
- **ServiceCollectionExtensions**: Metodi di registrazione servizi
- **ApplicationBuilderExtensions**: Metodi di configurazione middleware

## 🚀 Utilizzo

### 1. Aggiungi Reference al Progetto

```xml
<ProjectReference Include="..\Shared\RemaxApi.Shared.Authentication\RemaxApi.Shared.Authentication.csproj" />
```

### 2. Configura appsettings.json

```json
{
  "ExternalAuth": {
    "SecretKey": "chiave-segreta-condivisa-con-azienda-partner",
    "Issuer": "external-company-auth-service",
    "Audience": "remax-microservices"
  }
}
```

### 3. Registra i Servizi (Program.cs)

```csharp
using RemaxApi.Shared.Authentication.Extensions;

// Registra i servizi JWT
builder.Services.AddExternalJwtAuthentication();
```

### 4. Configura il Middleware

```csharp
using RemaxApi.Shared.Authentication.Extensions;

// Aggiungi il middleware (PRIMA di UseAuthentication)
app.UseExternalJwtValidation();
app.UseAuthentication();
app.UseAuthorization();
```

### 5. Usa nei Controller

```csharp
using RemaxApi.Shared.Authentication.Services;

[ApiController]
public class MyController : ControllerBase
{
    private readonly IExternalAuthUserService _userService;
    
    public MyController(IExternalAuthUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var userId = _userService.GetUserId();
        var userName = _userService.GetUserName();
        var roles = _userService.GetUserRoles();
        
        return Ok(new { userId, userName, roles });
    }
}
```

## 🔧 API Reference

### IExternalAuthUserService

| Metodo | Descrizione | Ritorna |
|--------|-------------|---------|
| `GetUserId()` | ID utente dal token | `string?` |
| `GetUserName()` | Nome utente | `string?` |
| `GetUserEmail()` | Email utente | `string?` |
| `GetUserRoles()` | Lista ruoli utente | `List<string>` |
| `GetAllClaims()` | Tutti i claims del token | `Dictionary<string, string>` |
| `IsAuthenticated()` | Stato autenticazione | `bool` |

## 📋 Claims JWT Supportati

La libreria gestisce automaticamente questi claims standard:

- **User ID**: `sub`, `user_id`, `NameIdentifier`
- **Username**: `name`, `username`, `Name`
- **Email**: `email`, `Email`
- **Roles**: `role`, `Role` (supporta valori multipli)

## 🔄 Aggiornamenti

Per aggiornare la libreria in tutti i microservizi:

1. Modifica il codice in `Shared/RemaxApi.Shared.Authentication/`
2. Ricompila i microservizi che la utilizzano
3. Tutti erediteranno automaticamente le modifiche

## 🐛 Troubleshooting

### Token non validato
- Verifica la configurazione `ExternalAuth:SecretKey`
- Controlla che `Issuer` e `Audience` corrispondano
- Verifica che il middleware sia registrato prima di `UseAuthentication()`

### Claims non trovati
- Controlla il formato del token JWT decodificato
- Verifica che i claims abbiano i nomi attesi
- Usa `GetAllClaims()` per debug

### Dipendenze mancanti
- Assicurati che i package NuGet necessari siano installati
- Verifica le versioni di compatibilità con .NET 8.0