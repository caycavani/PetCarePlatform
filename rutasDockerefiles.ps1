# Rutas esperadas por módulo
$modulos = @("Pets", "Booking", "Payment", "Review", "Notification", "Auth")
$raiz = Get-Location
$errores = 0

Write-Host "`nVerificando Dockerfiles por módulo..." -ForegroundColor Cyan

foreach ($modulo in $modulos) {
    $rutaEsperada = Join-Path $raiz $modulo
    $dockerfilePath = Join-Path $rutaEsperada "Dockerfile"

    if (-not (Test-Path $dockerfilePath)) {
        Write-Host "Falta Dockerfile en: $modulo/" -ForegroundColor Red
        $errores++
        continue
    }

    Write-Host "Dockerfile encontrado en: $modulo/" -ForegroundColor Green

    $contenido = Get-Content $dockerfilePath

    $sdkLine = $contenido | Where-Object { $_ -match "FROM mcr.microsoft.com/dotnet/sdk:" }
    $aspnetLine = $contenido | Where-Object { $_ -match "FROM mcr.microsoft.com/dotnet/aspnet:" }

    if ($sdkLine -match "9\.0") {
        Write-Host "$modulo usa SDK no recomendado: $sdkLine" -ForegroundColor Yellow
        $errores++
    }

    if ($aspnetLine -match "9\.0") {
        Write-Host "$modulo usa ASP.NET no recomendado: $aspnetLine" -ForegroundColor Yellow
        $errores++
    }
}

if ($errores -eq 0) {
    Write-Host "`nTodos los Dockerfiles están en su lugar y usan imágenes válidas." -ForegroundColor Green
} else {
    Write-Host "`nSe detectaron $errores problema(s). Revisa los mensajes anteriores." -ForegroundColor Yellow
}