# Ruta al archivo docker-compose.yml
$composePath = ".\docker-compose.yml"

if (-not (Test-Path $composePath)) {
    Write-Host "ERROR: No se encontro docker-compose.yml en el directorio actual." -ForegroundColor Red
    exit 1
}

Write-Host "`nVerificando rutas de contexto en docker-compose.yml..." -ForegroundColor Cyan

# Leer todas las líneas que contienen 'context:'
$lines = Select-String -Path $composePath -Pattern "context:" | ForEach-Object { $_.Line.Trim() }

if ($lines.Count -eq 0) {
    Write-Host "No se encontraron rutas de contexto en el archivo." -ForegroundColor Yellow
    exit 0
}

$errores = 0

foreach ($line in $lines) {
    $path = $line -replace "context:\s*", ""
    $normalizedPath = $path -replace "/", "\"
    if (Test-Path $normalizedPath) {
        Write-Host "Ruta valida: $path" -ForegroundColor Green
    } else {
        Write-Host "Ruta invalida: $path" -ForegroundColor Red
        $errores++
    }
}

if ($errores -eq 0) {
    Write-Host "`nTodas las rutas son validas. Ejecutando 'docker compose up --build -d'..." -ForegroundColor Green
    docker compose up --build -d
} else {
    Write-Host "`nSe encontraron $errores rutas invalidas. Corrige antes de continuar." -ForegroundColor Red
}