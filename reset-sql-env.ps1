Write-Host "🛑 Deteniendo contenedores..."
docker compose down

Write-Host "🧹 Intentando eliminar volumen..."
docker volume rm petcareplatform_sqlvolume 2>$null
Write-Host "🔔 Comando de eliminación ejecutado. Ignorando errores si el volumen no existe o está en uso."

Write-Host "🚀 Levantando entorno limpio..."
docker compose up -d

Write-Host "⏳ Esperando 15 segundos para que SQL Server arranque..."
Start-Sleep -Seconds 15

Write-Host "🔍 Revisando logs del contenedor SQL..."
$logs = docker logs petcare_sqlserver
Write-Host "📜 Últimas líneas:"
$logs | Select-String "init.sql" -SimpleMatch

Write-Host "⚠️ Ejecutando init.sql manualmente por si no se disparó automáticamente..."
docker exec -i petcare_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Nivacathy2033$#" -i /docker-entrypoint-initdb.d/init.sql

Write-Host "🏁 Proceso completo."
