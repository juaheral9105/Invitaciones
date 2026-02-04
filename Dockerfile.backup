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
COPY --from=build /app/backend/start.sh .

# Dar permisos de ejecución al script
RUN chmod +x start.sh

# Exponer puerto por defecto
EXPOSE 8080

# Usar el script de inicio que lee la variable PORT
CMD ["./start.sh"]
