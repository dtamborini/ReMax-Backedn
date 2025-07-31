# Test JWT Integration

Questo documento descrive come testare l'integrazione JWT tra UserService e AttachmentService.

## 1. Avvia i Servizi

### UserService (porta 8080)
```bash
cd UserService
dotnet run
```

### AttachmentService (porta 8081)
```bash
cd AttachmentService
dotnet run
```

## 2. Ottieni un Token JWT

### Opzione A: Endpoint Login JSON
```bash
curl -X POST "http://localhost:8080/api/externalauth/login" \
-H "Content-Type: application/json" \
-d '{
  "username": "admin",
  "password": "admin123",
  "clientId": "test-client"
}'
```

### Opzione B: Endpoint Token OAuth2
```bash
curl -X POST "http://localhost:8080/api/externalauth/token" \
-H "Content-Type: application/x-www-form-urlencoded" \
-d "grant_type=password&username=admin&password=admin123&client_id=test-client"
```

### Opzione C: Ottieni Token Pre-generati
```bash
curl -X GET "http://localhost:8080/api/externalauth/test-tokens"
```

## 3. Testa l'Autenticazione su AttachmentService

### Endpoint Pubblico (senza token)
```bash
curl -X GET "http://localhost:8081/api/authtest/health"
```

### Endpoint con Info Autenticazione (senza token)
```bash
curl -X GET "http://localhost:8081/api/authtest/anonymous"
```

### Endpoint Protetto (con token)
```bash
# Sostituisci YOUR_JWT_TOKEN con il token ottenuto sopra
curl -X GET "http://localhost:8081/api/authtest/protected" \
-H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Dettagli Utente (con token)
```bash
curl -X GET "http://localhost:8081/api/authtest/user-info" \
-H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 4. Utenti di Test Disponibili

| Username | Password | Ruoli |
|----------|----------|-------|
| admin | admin123 | admin, user |
| manager | manager123 | manager, user |
| user | user123 | user |
| testuser | testpassword | user, tester |

## 5. Esempio di Risposta JWT Decodificato

Il token JWT conterrà questi claims:
```json
{
  "sub": "user-001",
  "user_id": "user-001",
  "name": "Mario",
  "username": "admin",
  "email": "admin@remax.com",
  "role": ["admin", "user"],
  "iss": "external-company-auth-service",
  "aud": "remax-microservices",
  "exp": 1234567890,
  "iat": 1234567890,
  "jti": "uuid-here"
}
```

## 6. Debugging

### Log del UserService
- Controlla i log per vedere se il token viene generato correttamente
- Verifica che la configurazione ExternalAuth sia caricata

### Log del AttachmentService  
- Controlla i log per vedere se il token viene validato correttamente
- Verifica che i claims vengono estratti correttamente

### Errori Comuni
- **Token scaduto**: Verifica che l'ora del sistema sia corretta
- **Secret key diversa**: Assicurati che la chiave in ExternalAuth sia identica nei due servizi
- **Issuer/Audience**: Verifica che i valori corrispondano tra generazione e validazione

## 7. Produzione

Per l'ambiente di produzione:
1. Sostituisci la `SecretKey` con quella fornita dall'azienda partner
2. Aggiorna `Issuer` e `Audience` con i valori reali
3. Configura HTTPS per tutti gli endpoint
4. Implementa il refresh token se necessario