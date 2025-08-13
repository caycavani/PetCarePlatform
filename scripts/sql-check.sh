#!/bin/bash

# Configuración de conexión
SQL_SERVER="host.docker.internal"
SQL_PORT=1433
SQL_USER="petcare_user"
SQL_PASS="Nivacathy2033$#"
SQL_DB="master"

# Comando para verificar conexión
echo "🔍 Verificando conexión con SQL Server en $SQL_SERVER:$SQL_PORT..."

/opt/mssql-tools18/bin/sqlcmd -S $SQL_SERVER,$SQL_PORT -U $SQL_USER -P $SQL_PASS -Q "SELECT 1" > /dev/null

# Resultado
if [ $? -eq 0 ]; then
  echo "✅ Conexión exitosa con SQL Server desde el contenedor"
else
  echo "❌ Error: No se pudo conectar con SQL Server. Verifica que esté corriendo localmente y accesible desde Docker."
  exit 1
fi
