FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TenantService.csproj", "."]
RUN dotnet restore "./TenantService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./TenantService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TenantService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

# Installa EF tools per le migrazioni
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY --from=publish /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "TenantService.dll"]