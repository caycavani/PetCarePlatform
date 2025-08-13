#!/bin/bash
set -e

echo "▶ Instalando herramientas de SQL Server..."

# Actualiza apt y sus dependencias
apt-get update
apt-get install -y curl gnupg software-properties-common apt-transport-https

# Agrega la clave de Microsoft
curl -sSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /etc/apt/trusted.gpg.d/microsoft.gpg

# Agrega el repositorio
echo "deb [arch=amd64] https://packages.microsoft.com/ubuntu/20.04/prod focal main" > /etc/apt/sources.list.d/mssql-release.list

# Refresca lista de paquetes
apt-get update

# Instala los paquetes
ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools unixodbc-dev

# Agrega al PATH
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
export PATH="$PATH:/opt/mssql-tools/bin"

echo "✅ Herramientas instaladas exitosamente"
echo ""
echo "🔎 Verificando herramientas instaladas..."

if command -v sqlcmd >/dev/null 2>&1; then
    echo "✅ sqlcmd listo"
else
    echo "❌ sqlcmd no encontrado"
fi

if command -v bcp >/dev/null 2>&1; then
    echo "✅ bcp listo"
else
    echo "❌ bcp no encontrado"
fi
