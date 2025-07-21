$servicios = @{
    "auth-api"         = 5001
    "booking-api"      = 5002
    "notification-api" = 5003
    "payment-api"      = 5004
    "pets-api"         = 5005
    "review-api"       = 5006
}

foreach ($nombre in $servicios.Keys) {
    $puerto = $servicios[$nombre]
    $url = "http://localhost:$puerto/health"

    Write-Host "`nProbing $nombre en $url ..." -ForegroundColor Cyan

    try {
        $respuesta = Invoke-WebRequest -Uri $url -TimeoutSec 5 -UseBasicParsing
        if ($respuesta.StatusCode -eq 200) {
            Write-Host "$nombre respondió correctamente con código 200 ✅" -ForegroundColor Green
        } else {
            Write-Host "$nombre respondió con código $($respuesta.StatusCode) ⚠️" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "$nombre no respondió ➜ $($_.Exception.Message)" -ForegroundColor Red
    }
}
