$endpoint = "/health"
$servicios = @("auth-api", "booking-api", "notification-api", "payment-api", "pets-api", "review-api")
$contenedores = docker ps --format "{{.Names}}" | Where-Object { $_ -match "^petcareplatform-.*-api-1$" }

foreach ($origen in $contenedores) {
    Write-Host ""
    Write-Host "Desde: $origen"
    foreach ($destino in $servicios) {
        if ($origen -match $destino) { continue }

        $url = "http://$destino$endpoint"
        $cmd = "curl -s -o /dev/null -w `"Status: %{http_code}`" $url"
        $resultado = docker exec $origen sh -c "$cmd"

        Write-Host "Verificando $url desde $origen → $resultado"

        if ($resultado -match "Status: 200") {
            Write-Host "✅ $destino responde correctamente desde $origen"
        } elseif ($resultado -match "Status: 404") {
            Write-Host "⚠️ $destino responde 404 desde $origen"
        } elseif ($resultado -match "Could not resolve host") {
            Write-Host "❌ $destino no se resolvió desde $origen"
        } else {
            Write-Host "❌ Error de conexión desde $origen a $destino"
        }
    }
}