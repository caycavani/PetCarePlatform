# Lista de microservicios
$microservicios = @("Pets", "Booking", "Payment", "Review", "Notification", "Auth")
$dockerComposePath = ".\docker-compose.yml"
$errores = 0

Write-Host ""
Write-Host "Validando rutas de Dockerfile y configuración en docker-compose.yml..." -ForegroundColor Cyan

# Verifica existencia del docker-compose.yml
if (-not (Test-Path $dockerComposePath)) {
    Write-Host "ERROR: No se encontró docker-compose.yml en la ruta actual." -ForegroundColor Red
    exit 1
}

# Cargar contenido del docker-compose.yml
$composeContent = Get-Content $dockerComposePath -Raw

foreach ($ms in $microservicios) {
    $dockerfilePath = ".\$ms\Dockerfile"

    if (-not (Test-Path $dockerfilePath)) {
        Write-Host "ERROR: Falta Dockerfile en: $ms/" -ForegroundColor Red
        $errores++
    } else {
        Write-Host "OK: Dockerfile encontrado en: $ms/" -ForegroundColor Green
    }

    # Verificar que docker-compose.yml tenga el bloque correcto
    $expectedContext = "context: ./$ms"
    $expectedDockerfile = "dockerfile: Dockerfile"

    if ($composeContent -notmatch [regex]::Escape($expectedContext)) {
        Write-Host "ADVERTENCIA: docker-compose.yml no contiene: $expectedContext" -ForegroundColor Yellow
        $errores++
    }

    if ($composeContent -notmatch [regex]::Escape($expectedDockerfile)) {
        Write-Host "ADVERTENCIA: docker-compose.yml no contiene: $expectedDockerfile para $ms" -ForegroundColor Yellow
        $errores++
    }
}

if ($errores -eq 0) {
    Write-Host ""
    Write-Host "Todo está correctamente configurado." -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Se detectaron $errores problema(s). Revisa los mensajes anteriores." -ForegroundColor Yellow
}