# Imagen base para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Imagen para construir la app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar dependencias
COPY ["ComuniApp.Api.csproj", "."]
RUN dotnet restore "./ComuniApp.Api.csproj"

# Copiar todo el resto del proyecto y publicar
COPY . .
RUN dotnet publish "./ComuniApp.Api.csproj" -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ComuniApp.Api.dll"]
