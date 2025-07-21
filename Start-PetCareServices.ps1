# Ruta del docker-compose.yml
$composePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"

# Puertos asignados a los microservicios
$services = @{
    "auth-api" = 5001
    "booking-api" = 5002
    "notification-api" = 5003
    "payment-api" = 5004
    "pets-api" = 5005
    "review-api" = 5006
}

# Ejecutar docker-compose
Write-Host "🚀 Levantando contenedores con Docker Compose..." -ForegroundColor Cyan
cd $composePath
docker-compose up -d --build

# Espera para inicialización
Write-Host "⏳ Esperando a que los contenedores se estabilicen..." -ForegroundColor Yellow
Start-Sleep -Seconds 20

# Verificar contenedores activos
Write-Host "🔍 Verificando contenedores activos:" -ForegroundColor Cyan
docker ps | Select-String "petcare"

# Verificar conectividad de servicios
Write-Host "`n📡 Probando servicios expuestos en localhost:" -ForegroundColor Cyan

foreach ($name in $services.Keys) {
    $port = $services[$name]
    $url = "http://localhost:$port"

    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ $name responde correctamente en puerto $port" -ForegroundColor Green
        } else {
            Write-Host "⚠️ $name responde con código HTTP $($response.StatusCode)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "❌ No se pudo conectar con $name en puerto $port" -ForegroundColor Red
    }
}

Write-Host "`n🎉 Todos los servicios han sido verificados." -ForegroundColor Cyan