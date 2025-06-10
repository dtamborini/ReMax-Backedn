# MappingService/Dockerfile

# Vedi https://aka.ms/customizecontainer per imparare come personalizzare
# il tuo container di debug e come Visual Studio usa questo Dockerfile
# per costruire le tue immagini per un debugging più veloce.

# Questa fase viene usata quando si esegue da VS in modalità fast (Default per la configurazione Debug)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Questa fase viene usata per costruire il progetto del servizio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copia il file .csproj del MappingService e ripristina le dipendenze.
# Questo è ottimizzato per la cache di Docker: se il .csproj non cambia,
# lo strato di ripristino non viene ricostruito.
COPY ["MappingService/MappingService.csproj", "MappingService/"]
RUN dotnet restore "./MappingService/MappingService.csproj"

# Copia il resto dei file del progetto MappingService
COPY . .
WORKDIR "/src/MappingService"
RUN dotnet build "./MappingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Questa fase viene usata per pubblicare il progetto del servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MappingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Questa fase viene usata in produzione o quando si esegue da VS in modalità regolare
# (Default quando non si usa la configurazione Debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MappingService.dll"]