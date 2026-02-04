# Usar la imagen oficial de .NET 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copiar el proyecto y restaurar dependencias
COPY backend/*.csproj ./backend/
WORKDIR /app/backend
RUN dotnet restore

# Copiar el resto del código y compilar
COPY backend/ ./
RUN dotnet publish -c Release -o out

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/backend/out .

# Exponer puerto por defecto (Railway lo sobreescribirá)
EXPOSE 8080

# El puerto se configura mediante la variable de entorno ASPNETCORE_URLS
# Railway la establecerá automáticamente
ENTRYPOINT ["dotnet", "InvitacionesAPI.dll"]
