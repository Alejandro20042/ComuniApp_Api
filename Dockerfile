# -------------------------------
# Dockerfile para ComuniApp.Api
# -------------------------------

# Imagen base para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Imagen para construir la app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar el .csproj y restaurar dependencias
COPY ["ComuniApp.Api.csproj", "."]
RUN dotnet restore "ComuniApp.Api.csproj"

# Copiar todo el proyecto y publicar en /app/publish
COPY . .
RUN dotnet publish "ComuniApp.Api.csproj" -c Release -o /app/publish

# Imagen final para producción
FROM base AS final
WORKDIR /app

# Copiar la app publicada desde la fase build
COPY --from=build /app/publish .

# Ejecutar la app
ENTRYPOINT ["dotnet", "ComuniApp.Api.dll"]

