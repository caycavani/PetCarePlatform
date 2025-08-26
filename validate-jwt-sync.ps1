# Microservicios y sus endpoints de diagnóstico
$services = @{
    "auth-api"    = "http://localhost:5001/api/debug/jwt-config"
    "review-api"  = "http://localhost:5002/api/debug/jwt-config"
    "booking-api" = "http://localhost:5003/api/debug/jwt-config"
}

# Valores esperados
$expectedIssuer = "http://petcare_auth"
$expectedAudience = "PetCareClientApp"
$expectedSecretLength = 32  # Ajusta según tu clave real

Write-Host ""
Write-Host "Validando configuración JWT en microservicios..." -ForegroundColor Cyan

foreach ($name in $services.Keys) {
    $url = $services[$name]
    Write-Host ""
    Write-Host "$name ($url)" -ForegroundColor Yellow

    try {
        $response = Invoke-RestMethod -Uri $url -Method Get

        $issuer = $response.issuer
        $audience = $response.audience
        $secretLength = $response.secretLength

        if ($issuer -eq $expectedIssuer) {
            Write-Host "Issuer OK" -ForegroundColor Green
        } else {
            Write-Host "Issuer mismatch: $issuer" -ForegroundColor Red
        }

        if ($audience -eq $expectedAudience) {
            Write-Host "Audience OK" -ForegroundColor Green
        } else {
            Write-Host "Audience mismatch: $audience" -ForegroundColor Red
        }

        if ($secretLength -ge $expectedSecretLength) {
            Write-Host "Secret length OK ($secretLength)" -ForegroundColor Green
        } else {
            Write-Host "Secret length too short: $secretLength" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "Error al consultar `${name}: $($_.Exception.Message)" -ForegroundColor Magenta
    }
}

Write-Host ""
Write-Host "Validación completada." -ForegroundColor Cyan
