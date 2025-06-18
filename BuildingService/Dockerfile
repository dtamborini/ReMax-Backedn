# BuildingService/Dockerfile

# Questa fase viene usata quando si esegue da VS in modalità fast (Default per la configurazione Debug)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Questa fase viene usata per costruire il progetto del servizio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BuildingService/BuildingService.csproj", "BuildingService/"]
RUN dotnet restore "./BuildingService/BuildingService.csproj"

# Copia il resto dei file del progetto BuildingService
COPY . .
WORKDIR "/src/BuildingService"
RUN dotnet build "./BuildingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Questa fase viene usata per pubblicare il progetto del servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BuildingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Questa fase viene usata in produzione o quando si esegue da VS in modalità regolare
# (Default quando non si usa la configurazione Debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BuildingService.dll"]