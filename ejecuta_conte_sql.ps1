# Configuración
$server = "localhost"
$user = "sa"
$password = "Nivacathy2033`$#"

# Ruta del archivo SQL
$sqlFile = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Auth\PetCare.Auth.Api\sql\init-auth.sql"

# Verificación de existencia del archivo
if (-Not (Test-Path $sqlFile)) {
    Write-Host "❌ El archivo '$sqlFile' no fue encontrado." -ForegroundColor Red
    exit 1
}

# Esperar a que SQL Server esté listo
Write-Host "⏳ Esperando que SQL Server esté listo..."
Start-Sleep -Seconds 15

# Ejecutar el script SQL
Write-Host "🚀 Ejecutando '$sqlFile'..."
try {
    $output = sqlcmd -S $server -U $user -P $password -i $sqlFile 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Script ejecutado correctamente." -ForegroundColor Green
        Write-Host $output
    } else {
        Write-Host "❌ Error al ejecutar el script." -ForegroundColor Red
        Write-Host $output
        exit $LASTEXITCODE
    }
} catch {
    Write-Host "❌ Excepción al ejecutar sqlcmd: $_" -ForegroundColor Red
    exit 1
}
