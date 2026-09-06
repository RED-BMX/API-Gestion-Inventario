# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY API-Gestion-Inventario.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish API-Gestion-Inventario.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "API-Gestion-Inventario.dll"]
