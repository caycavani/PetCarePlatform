# Ruta al archivo docker-compose.yml
$composePath = ".\docker-compose.yml"

# Verifica que el archivo exista
if (-not (Test-Path $composePath)) {
    Write-Host "ERROR: No se encontró docker-compose.yml en la ruta actual." -ForegroundColor Red
    exit 1
}

# Cargar contenido YAML como texto
$lines = Get-Content $composePath
$currentService = ""
$context = ""
$dockerfile = ""
$errores = 0

Write-Host ""
Write-Host "Verificando rutas de Dockerfile definidas en docker-compose.yml..." -ForegroundColor Cyan

foreach ($line in $lines) {
    $trimmed = $line.Trim()

    if ($trimmed -match "^[a-zA-Z0-9\-]+:$" -and $line.StartsWith("  ") -eq $false) {
        $currentService = $trimmed.TrimEnd(":")
        $context = ""
        $dockerfile = ""
    }

    if ($trimmed -like "context:*") {
        $context = $trimmed -replace "context:\s*", ""
    }

    if ($trimmed -like "dockerfile:*") {
        $dockerfile = $trimmed -replace "dockerfile:\s*", ""

        # Validar ruta combinada
        $rutaCompleta = Join-Path $context $dockerfile
        if (-not (Test-Path $rutaCompleta)) {
            Write-Host ("ERROR: {0} - No se encontró el Dockerfile en '{1}'" -f $currentService, $rutaCompleta) -ForegroundColor Red
            $errores++
        } else {
            Write-Host ("OK: {0} - Dockerfile encontrado en '{1}'" -f $currentService, $rutaCompleta) -ForegroundColor Green
        }
    }
}

if ($errores -eq 0) {
    Write-Host ""
    Write-Host "Todo está correctamente configurado." -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host ("Se detectaron {0} problema(s) con rutas de Dockerfile." -f $errores) -ForegroundColor Yellow
}