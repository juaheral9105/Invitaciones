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

# Configurar el puerto (Railway usa la variable PORT)
ENV ASPNETCORE_URLS=http://+:$PORT

ENTRYPOINT ["dotnet", "InvitacionesAPI.dll"]
