#!/bin/sh
# Script de inicio para Railway
# Lee el puerto de la variable de entorno PORT o usa 8080 por defecto
export ASPNETCORE_URLS="http://*:${PORT:-8080}"
echo "Starting application on port ${PORT:-8080}"
echo "=== DEBUG: Environment variables ==="
echo "PORT: ${PORT}"
echo "DATABASE_URL exists: $(test -n "$DATABASE_URL" && echo "yes" || echo "no")"
echo "DATABASE_PUBLIC_URL exists: $(test -n "$DATABASE_PUBLIC_URL" && echo "yes" || echo "no")"
echo "ConnectionStrings__DefaultConnection exists: $(test -n "$ConnectionStrings__DefaultConnection" && echo "yes" || echo "no")"
echo "==================================="
exec dotnet InvitacionesAPI.dll
