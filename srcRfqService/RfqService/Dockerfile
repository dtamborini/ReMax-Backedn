FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RfqService.csproj", "."]
COPY ["../../srcShared/CondominiumManagement.Shared/CondominiumManagement.Shared.csproj", "../srcShared/CondominiumManagement.Shared/"]
RUN dotnet restore "./RfqService.csproj"
COPY . .
COPY ../../srcShared/ ../srcShared/
WORKDIR "/src/."
RUN dotnet build "./RfqService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RfqService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RfqService.dll"]