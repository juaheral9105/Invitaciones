#!/bin/sh
# Script de inicio para Railway
# Lee el puerto de la variable de entorno PORT o usa 8080 por defecto
export ASPNETCORE_URLS="http://*:${PORT:-8080}"
echo "Starting application on port ${PORT:-8080}"
exec dotnet InvitacionesAPI.dll
