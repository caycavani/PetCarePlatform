Set-Location "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"

Write-Host ""
Write-Host "Reconstruyendo imagen base..."
docker build --no-cache -f Base/Base.Api.Dockerfile -t petcareplatform/base-api:8.0 .

Write-Host ""
Write-Host "Reconstruyendo microservicios..."
docker-compose build --no-cache

Write-Host ""
Write-Host "Iniciando servicios..."
docker-compose down
docker-compose up -d

Write-Host ""
Write-Host "Verificando curl en contenedores..."

$contenedores = docker ps --format "{{.Names}}"
foreach ($nombre in $contenedores) {
    Write-Host ""
    Write-Host "Contenedor: $nombre"
    $resultado = docker exec $nombre bash -c "curl --version" 2>&1
    if ($resultado -match "curl\s+[0-9\.]+") {
        Write-Host "curl disponible"
    } else {
        Write-Host "curl NO disponible"
    }
}

Write-Host ""
Write-Host "Verificando conectividad a /health desde contenedor externo..."

$diagnosticoId = docker run -d --network petcareplatform_petcare-net petcareplatform/base-api:8.0 tail -f /dev/null
Start-Sleep -Seconds 3

$servicios = @(
    "petcareplatform-auth-api-1",
    "petcareplatform-booking-api-1",
    "petcareplatform-notification-api-1",
    "petcareplatform-payment-api-1",
    "petcareplatform-pets-api-1",
    "petcareplatform-review-api-1"
)

foreach ($servicio in $servicios) {
    Write-Host ""
    Write-Host "Probing: http://$servicio/health"
    $respuesta = docker exec $diagnosticoId bash -c "curl --max-time 3 -s -o /dev/null -w '%{http_code}' http://$servicio/health" 2>&1
    if ($respuesta -eq "200") {
        Write-Host "$servicio respondió correctamente (HTTP 200)"
    } else {
        Write-Host "$servicio NO respondió correctamente (Código: $respuesta)"
    }
}

docker rm -f $diagnosticoId | Out-Null
