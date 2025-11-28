# ----------------------------------------
# Dockerfile para ComuniApp.Api (Render)
# ----------------------------------------

# Imagen base para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Imagen para construir la app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar solo el .csproj y restaurar dependencias
COPY ["ComuniApp.Api.csproj", "."]
RUN dotnet restore "ComuniApp.Api.csproj"

# Copiar todo el proyecto y publicar
COPY . .
RUN dotnet publish "ComuniApp.Api.csproj" -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app

# Copiar desde la etapa de build
COPY --from=build /app/publish .

# Ejecutar la app
ENTRYPOINT ["dotnet", "ComuniApp.Api.dll"]
