# Imagen base de ASP.NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Imagen para construir
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ComuniApp_Api.csproj", "."]
RUN dotnet restore "./ComuniApp_Api.csproj"
COPY . .
RUN dotnet publish "./ComuniApp_Api.csproj" -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ComuniApp_Api.dll"]
