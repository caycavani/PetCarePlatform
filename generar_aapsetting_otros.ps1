param (
    [int]$PuertoBase = 5000,
    [string[]]$Servicios = @("auth.api", "pets.api", "booking.api", "review.api", "notification.api", "payment.api")
)

Write-Host "`n🚀 Iniciando docker-compose..." -ForegroundColor Cyan
docker-compose up -d

Start-Sleep -Seconds 8

$index = 0

foreach ($servicio in $Servicios) {
    $puerto = $PuertoBase + $index
    $index++
    $baseUrl = "http://localhost:$puerto"

    Write-Host "`n▶️ Verificando $servicio ($baseUrl)" -ForegroundColor Yellow

    try {
        $swagger = Invoke-WebRequest -Uri "$baseUrl/swagger" -UseBasicParsing -TimeoutSec 5
        Write-Host "   ✅ Swagger: OK" -ForegroundColor Green
    }
    catch {
        Write-Host "   ❌ Swagger: No responde" -ForegroundColor Red
    }

    try {
        $health = Invoke-WebRequest -Uri "$baseUrl/health" -UseBasicParsing -TimeoutSec 5
        Write-Host "   ✅ Health: OK" -ForegroundColor Green
    }
    catch {
        Write-Host "   ❌ Health: No responde" -ForegroundColor Red
    }
}

Write-Host "`n🎯 Verificación finalizada. Revisa servicios que no respondieron." -ForegroundColor Cyan