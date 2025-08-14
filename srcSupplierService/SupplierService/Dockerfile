FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["srcSupplierService/SupplierService/SupplierService.csproj", "srcSupplierService/SupplierService/"]
COPY ["srcShared/RemaxManagement.Shared/RemaxManagement.Shared.csproj", "srcShared/RemaxManagement.Shared/"]
RUN dotnet restore "srcSupplierService/SupplierService/SupplierService.csproj"
COPY . .
WORKDIR "/src/srcSupplierService/SupplierService"
RUN dotnet build "SupplierService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SupplierService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SupplierService.dll"]