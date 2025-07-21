param (
    [string]$ComposePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\docker-compose.yml"
)

if (-not (Test-Path $ComposePath)) {
    Write-Host "❌ No se encontró el archivo docker-compose.yml en la ruta especificada." -ForegroundColor Red
    exit 1
}

# Leer y filtrar líneas
$lines = Get-Content $ComposePath
$filtered = $lines | Where-Object { -not ($_ -match '^\s*version\s*:\s*') }

# Sobrescribir el archivo sin la línea version
Set-Content -Path $ComposePath -Value $filtered -Encoding UTF8

Write-Host "`n✅ Línea 'version:' eliminada de docker-compose.yml correctamente." -ForegroundColor Green