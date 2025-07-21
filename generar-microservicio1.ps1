param (
    [string]$RutaBase = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform",
    [string[]]$Microservicios = @("Auth", "Pets", "Booking", "Review", "Notification", "Payment")
)

Write-Host "`nVerificando compilacion de microservicios..." -ForegroundColor Cyan

foreach ($ms in $Microservicios) {
    $ruta = Join-Path $RutaBase $ms
    $dockerfile = Join-Path $ruta "Dockerfile"

    if (-not (Test-Path $dockerfile)) {
        Write-Host "No se encontro Dockerfile en: $ms" -ForegroundColor Red
        continue
    }

    Write-Host "`nCompilando $ms..." -ForegroundColor Yellow

    $resultado = docker build "$ruta" -f "$dockerfile" --no-cache 2>&1

    if ($LASTEXITCODE -eq 0) {
        Write-Host "$ms compilado exitosamente." -ForegroundColor Green
    }
    else {
        Write-Host "Fallo al compilar $ms" -ForegroundColor Red
        $logPath = Join-Path $RutaBase "build-$ms.log"
        $resultado | Out-File -FilePath $logPath -Encoding UTF8
        Write-Host "Log guardado en: $logPath"
    }
}

Write-Host "`nVerificacion de compilacion finalizada." -ForegroundColor Cyan