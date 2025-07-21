# Microservicios a verificar
$microservicios = @(
    @{ Nombre = "auth-api";     Ruta = "Auth/PetCare.Auth.Api";           Dll = "PetCare.Auth.Api.dll" },
    @{ Nombre = "booking-api";  Ruta = "Booking/PetCare.Booking.Api";     Dll = "PetCare.Booking.Api.dll" },
    @{ Nombre = "notification"; Ruta = "Notification/PetCare.Notification.Api"; Dll = "PetCare.Notification.Api.dll" },
    @{ Nombre = "payment-api";  Ruta = "Payment/PetCare.Payment.Api";     Dll = "PetCare.Payment.Api.dll" },
    @{ Nombre = "pets-api";     Ruta = "Pets/PetCare.Pets.Api";           Dll = "PetCare.Pets.Api.dll" },
    @{ Nombre = "review-api";   Ruta = "Review/PetCare.Review.Api";       Dll = "PetCare.Review.Api.dll" }
)

# Ruta base de la solución
$solucion = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"

# Ruta del archivo de log
$logPath = "$solucion\verificacion_microservicios_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"

foreach ($svc in $microservicios) {
    $nombre     = $svc.Nombre
    $proyecto   = Join-Path $solucion $svc.Ruta
    $publishDir = Join-Path $proyecto "temp_publish"
    $dllPath    = Join-Path $publishDir $svc.Dll
    $endpoint   = "http://localhost:80/health"

    Write-Host "`nVerificando: $nombre" -ForegroundColor Cyan
    Add-Content $logPath "`n[$nombre] Verificando $endpoint"

    dotnet publish $proyecto -c Release -o $publishDir | Out-Null

    if (!(Test-Path $dllPath)) {
        $msg = "❌ No se encontró el archivo DLL para $nombre"
        Write-Host $msg -ForegroundColor Red
        Add-Content $logPath $msg
        continue
    }

    # Forzar que la app escuche en puerto 80
    $env:ASPNETCORE_URLS = "http://+:80"

    # Ejecutar la API temporalmente
    $proc = Start-Process -FilePath "dotnet" -ArgumentList "`"$dllPath`"" -WorkingDirectory $publishDir -PassThru
    Start-Sleep -Seconds 5

    # Probar conexión al endpoint /health
    try {
        $response = Invoke-WebRequest -Uri $endpoint -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            $msg = "✅ $nombre responde correctamente en $endpoint"
            Write-Host $msg -ForegroundColor Green
        } else {
            $msg = "⚠️ $nombre respondió con código HTTP $($response.StatusCode)"
            Write-Host $msg -ForegroundColor Yellow
        }
        Add-Content $logPath $msg
    } catch {
        $msg = "❌ No se pudo conectar con $nombre en $endpoint"
        Write-Host $msg -ForegroundColor Red
        Add-Content $logPath $msg
    }

    # Detener proceso y limpiar
    Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    if (Test-Path $publishDir) {
        Remove-Item $publishDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# ✅ Cierre correcto del script
Write-Host "`nVerificación completada. Resultados guardados en:`n$logPath" -ForegroundColor Cyan