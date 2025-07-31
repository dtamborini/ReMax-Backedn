# Guida per Testare JWT tramite Swagger

## 🚀 **Passo 1: Avvia i Servizi**

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

## 🔑 **Passo 2: Ottieni un JWT Token**

### Opzione A: Tramite curl
```bash
curl -X POST "http://localhost:8080/api/externalauth/login" \
-H "Content-Type: application/json" \
-d '{
  "username": "admin",
  "password": "admin123",
  "clientId": "test-client"
}'
```

### Opzione B: Tramite Swagger UserService
1. Vai su `http://localhost:8080/swagger`
2. Usa l'endpoint `POST /api/externalauth/login`
3. Copia il `access_token` dalla risposta

## 🛡️ **Passo 3: Testa con Swagger AttachmentService**

1. **Apri Swagger** AttachmentService: `http://localhost:8081/swagger`

2. **Clicca sul pulsante "Authorize"** (icona lucchetto in alto a destra)

3. **Inserisci il token** nel campo "Value":
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```
   ⚠️ **IMPORTANTE**: Devi includere la parola "Bearer " prima del token!

4. **Clicca "Authorize"** poi "Close"

5. **Testa gli endpoint** protetti:
   - `GET /api/authtest/protected` - Endpoint di test
   - `GET /api/authtest/user-info` - Info utente completa
   - `GET /api/buildings/{uuidBuilding}/worksheets/{uuidWorksheet}/rfqs/{uuidRfq}/quotes/{uuidQuote}/attachments` - Endpoint AttachmentQuote

## 📋 **Passo 4: Verifica la Risposta**

Gli endpoint del controller AttachmentQuote ora restituiscono:

```json
{
  "data": [/* attachment data */],
  "userInfo": {
    "userId": "user-001",
    "userName": "Mario",
    "userEmail": "admin@remax.com", 
    "userRoles": ["admin", "user"],
    "isAuthenticated": true
  },
  "timestamp": "2025-01-31T10:30:00.000Z"
}
```

## 🧪 **Token di Test Disponibili**

Puoi anche ottenere token pre-generati da:
```bash
curl -X GET "http://localhost:8080/api/externalauth/test-tokens"
```

## ✅ **Cosa Verificare**

1. **Senza token**: Gli endpoint protetti restituiscono 401 Unauthorized
2. **Con token valido**: Gli endpoint restituiscono 200 OK con `userInfo` popolata
3. **Con token scaduto/non valido**: Gli endpoint restituiscono 401 Unauthorized
4. **Info utente**: Verifica che `userId`, `userName`, `userRoles` siano corretti

## 🐛 **Troubleshooting**

### Errore 401 con token valido
- Verifica di aver incluso "Bearer " prima del token
- Controlla che la `SecretKey` sia identica nei due servizi
- Verifica che i servizi utilizzino la stessa configurazione `ExternalAuth`

### Token non viene validato
- Controlla i log del AttachmentService per errori di validazione
- Verifica che il middleware JWT sia registrato correttamente
- Controlla che `Issuer` e `Audience` corrispondano

### Swagger non mostra il pulsante Authorize
- Verifica che la configurazione Bearer sia corretta nel Program.cs
- Riavvia il servizio AttachmentService