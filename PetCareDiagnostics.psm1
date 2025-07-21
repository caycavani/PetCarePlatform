function Test-MicroserviceStructure {
    param (
        [string]$BasePath,
        [hashtable[]]$Microservicios
    )

    foreach ($svc in $Microservicios) {
        $nombre = $svc.Nombre
        $programPath = Join-Path $BasePath "$($svc.Ruta)\Program.cs"

        Write-Host "`n📦 [$nombre] - Validando Program.cs"

        if (!(Test-Path $programPath)) {
            Write-Host "❌ Program.cs no encontrado"
            continue
        }

        $contenido = Get-Content $programPath -Raw
        $checks = @(
            @{ Texto = 'UseUrls\s*\(\s*"http://\*:80"\s*\)'; Ok = '✅ Usa puerto 80'; Fail = '⚠️ FALTA: UseUrls("http://*:80")' },
            @{ Texto = 'AddHealthChecks\s*\('; Ok = '✅ AddHealthChecks registrado'; Fail = '⚠️ FALTA: AddHealthChecks()' },
            @{ Texto = 'MapHealthChecks\s*\(\s*"/health"\s*\)'; Ok = '✅ MapHealthChecks("/health")'; Fail = '⚠️ FALTA: MapHealthChecks("/health")' },
            @{ Texto = 'UseRouting\s*\('; Ok = '✅ Usa UseRouting()'; Fail = '⚠️ FALTA: UseRouting()' },
            @{ Texto = 'MapControllers\s*\('; Ok = '✅ Usa MapControllers()'; Fail = '⚠️ FALTA: MapControllers()' }
        )

        foreach ($check in $checks) {
            if ($contenido -match $check.Texto) {
                Write-Host $check.Ok
            } else {
                Write-Host $check.Fail
            }
        }
    }
}

function Test-DockerConfiguration {
    param (
        [string]$ComposePath,
        [hashtable[]]$Microservicios
    )

    Import-Module powershell-yaml
    $compose = ConvertFrom-Yaml (Get-Content $ComposePath -Raw)

    foreach ($svc in $Microservicios) {
        $nombre = $svc.Nombre
        $composeKey = $svc.ComposeKey

        Write-Host "`n🐳 [$nombre] - docker-compose.yml"

        if (-not $compose.services.ContainsKey($composeKey)) {
            Write-Host "❌ Servicio $composeKey no definido"
            continue
        }

        $env = $compose.services[$composeKey].environment
        if ($env -contains "ASPNETCORE_ENVIRONMENT=Development") {
            Write-Host "✅ Entorno Development definido"
        } else {
            Write-Host "⚠️ FALTA: ASPNETCORE_ENVIRONMENT=Development"
        }

        $puertos = $compose.services[$composeKey].ports
        if ($puertos -and $puertos.Count -gt 0) {
            $exp = $puertos[0].Split(":")[0]
            Write-Host "✅ Puerto expuesto: $exp"
        } else {
            Write-Host "⚠️ FALTA: No se define puerto"
        }
    }
}

function Test-HealthConnectivity {
    param (
        [string[]]$Servicios,
        [string]$MatchPrefix = "petcareplatform-(.*)"
    )

    $endpoint = "/health"
    $contenedoresActivos = docker ps --format "{{.Names}}" | Where-Object { $_ -match $MatchPrefix }

    foreach ($origen in $contenedoresActivos) {
        Write-Host "`n📡 Desde: $origen"

        foreach ($destino in $Servicios) {
            if ($origen -match $destino) { continue }

            $url = "http://$destino$endpoint"
            $cmd = "curl -s -o /dev/null -w `"Status: %{http_code}`" $url"

            try {
                $resultado = docker exec $origen sh -c "$cmd"
            } catch {
                Write-Host "❌ Error desde $origen a $destino"
                continue
            }

            if ($resultado -match "Status: 200") {
                Write-Host "✅ [$destino] responde desde [$origen]"
            } elseif ($resultado -match "Status: 404") {
                Write-Host "⚠️ [$destino] responde 404 desde [$origen]"
            } elseif ($resultado -match "Could not resolve host") {
                Write-Host "❌ [$destino] no se resolvió desde [$origen]"
            } else {
                Write-Host "❌ [$destino] error desde [$origen] ($resultado)"
            }
        }
    }
}

function Run-AllDiagnostics {
    $BasePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"
    $ComposePath = Join-Path $BasePath "docker-compose.yml"

    $Microservicios = @(
        @{ Nombre = "auth-api"; Ruta = "Auth/PetCare.Auth.Api"; ComposeKey = "auth-api" },
        @{ Nombre = "booking-api"; Ruta = "Booking/PetCare.Booking.Api"; ComposeKey = "booking-api" },
        @{ Nombre = "notification"; Ruta = "Notification/PetCare.Notification.Api"; ComposeKey = "notification-api" },
        @{ Nombre = "payment-api"; Ruta = "Payment/PetCare.Payment.Api"; ComposeKey = "payment-api" },
        @{ Nombre = "pets-api"; Ruta = "Pets/PetCare.Pets.Api"; ComposeKey = "pets-api" },
        @{ Nombre = "review-api"; Ruta = "Review/PetCare.Review.Api"; ComposeKey = "review-api" }
    )

    $Servicios = $Microservicios | ForEach-Object { $_.ComposeKey }

    Write-Host "`n🧪 Ejecutando Test-MicroserviceStructure"
    Test-MicroserviceStructure -BasePath $BasePath -Microservicios $Microservicios

    Write-Host "`n🐳 Ejecutando Test-DockerConfiguration"
    Test-DockerConfiguration -ComposePath $ComposePath -Microservicios $Microservicios

    Write-Host "`n📡 Ejecutando Test-HealthConnectivity"
    Test-HealthConnectivity -Servicios $Servicios
}