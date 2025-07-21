$endpoints = @{
    "Pets API"         = "http://localhost:5001/health"
    "Booking API"      = "http://localhost:5002/health"
    "Payment API"      = "http://localhost:5003/health"
    "Review API"       = "http://localhost:5004/health"
    "Notification API" = "http://localhost:5005/health"
}

$logPath = ".\healthcheck.log"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
Add-Content -Path $logPath -Value "`n[$timestamp] Iniciando verificacion de endpoints /health..."

Write-Host "`nVerificando estado de los endpoints /health..." -ForegroundColor Cyan

foreach ($name in $endpoints.Keys) {
    $url = $endpoints[$name]
    $success = $false

    for ($i = 1; $i -le 3; $i++) {
        try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 5
            if ($response.StatusCode -eq 200) {
                Write-Host "$name responde correctamente (200 OK)" -ForegroundColor Green
                Add-Content -Path $logPath -Value "[$timestamp] $name OK (200) en intento $i"
                $success = $true
                break
            } else {
                Write-Host "$name respondio con codigo $($response.StatusCode) (intento $i)" -ForegroundColor Yellow
                Add-Content -Path $logPath -Value "[$timestamp] $name codigo $($response.StatusCode) en intento $i"
            }
        } catch {
            Write-Host "$name no respondio (intento $i)" -ForegroundColor Red
            Add-Content -Path $logPath -Value "[$timestamp] $name no respondio en intento $i"
        }

        Start-Sleep -Seconds 2
    }

    if (-not $success) {
        Write-Host "$name fallo despues de 3 intentos." -ForegroundColor Red
        Add-Content -Path $logPath -Value "[$timestamp] $name fallo despues de 3 intentos."
    }
}

Write-Host "`nResultados registrados en $logPath" -ForegroundColor Cyan