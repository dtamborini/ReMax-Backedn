# Service Configuration Guide

## Service URLs Configuration

### Development (Local)
When running services locally on your development machine:

| Service | Local URL | Port |
|---------|-----------|------|
| Authentication Service | http://localhost:5001 | 5001 |
| Building Service | http://localhost:5002 | 5002 |
| WorkQuote Service | http://localhost:5003 | 5003 |
| User Service | http://localhost:5004 | 5004 |
| RFQ Service | http://localhost:5005 | 5005 |
| WorkSheet Service | http://localhost:5006 | 5006 |
| Communication Service | http://localhost:5007 | 5007 |
| Tenant Service | http://localhost:5008 | 5008 |
| Attachment Service | http://localhost:5009 | 5009 |
| Insurance Service | http://localhost:5010 | 5010 |
| Supplier Service | http://localhost:5011 | 5011 |
| Deadline Service | http://localhost:5012 | 5012 |
| Maintenance Plan Service | http://localhost:5013 | 5013 |
| State Service | http://localhost:5014 | 5014 |
| API Gateway | http://localhost:5000 | 5000 |

### Docker Compose (Container Network)
When running services in Docker containers:

| Service | Internal URL | Internal Port |
|---------|-------------|---------------|
| All Services | http://{service-name}:8080 | 8080 |
| PostgreSQL | postgres-db:5432 | 5432 |

#### Example Internal URLs:
- Building → Attachment: `http://attachment-service:8080`
- Any Service → PostgreSQL: `Host=postgres-db;Port=5432`

## Configuration Files

### appsettings.json (Development)
```json
{
  "Services": {
    "AttachmentService": {
      "Url": "http://localhost:5009"
    }
  }
}
```

### appsettings.Production.json (Docker/Production)
```json
{
  "Services": {
    "AttachmentService": {
      "Url": "http://attachment-service:8080"
    }
  }
}
```

### docker-compose.yml Environment Variables
```yaml
environment:
  - Services__AttachmentService__Url=http://attachment-service:8080
```

## Service Discovery

### Local Development
- Services communicate via localhost on their exposed ports
- Each service runs on a different port

### Docker Environment
- Services communicate via service names defined in docker-compose.yml
- All services listen on port 8080 internally
- Docker's internal DNS resolves service names to container IPs
- External access is via mapped ports (e.g., 5002:8080)

## Best Practices

1. **Environment Variables Override**: Docker environment variables override appsettings.json
   ```
   Priority: Environment Variable > appsettings.Production.json > appsettings.json
   ```

2. **Service Dependencies**: Define service dependencies in docker-compose.yml
   ```yaml
   depends_on:
     postgres-db:
       condition: service_healthy
     attachment-service:
       condition: service_started
   ```

3. **Health Checks**: Implement health check endpoints
   ```csharp
   app.MapGet("/health", () => "Service is running")
   ```

4. **Resilient Communication**: Implement retry policies for inter-service calls
   ```csharp
   services.AddHttpClient("AttachmentService")
           .AddPolicyHandler(GetRetryPolicy());
   ```

## Troubleshooting

### Service Cannot Connect to Another Service
1. Check service name in URL matches docker-compose.yml
2. Verify both services are in the same network
3. Check service dependencies are correctly defined
4. Review logs: `docker-compose logs {service-name}`

### Different URL in Different Environments
Use configuration hierarchy:
- Local: appsettings.json
- Docker: Environment variables in docker-compose.yml
- Production: appsettings.Production.json or environment variables

### Testing Inter-Service Communication
```bash
# From inside a container
docker exec -it {container-name} /bin/bash
curl http://attachment-service:8080/health

# From host machine
curl http://localhost:5009/health
```